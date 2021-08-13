using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class ConfigureEnvironment
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = ConfigureScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to prepare for workload installation?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task ConfigureScaleUnit(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                if (ScaleUnitContext.GetScaleUnitId() == "@@")
                    await new HubConfigurationManager().Configure();
                else
                    await new ScaleUnitConfigurationManager().Configure();
            }
        }
    }
}
