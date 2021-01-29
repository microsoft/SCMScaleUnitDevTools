using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class ConfigureEnvironment
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = ConfigureScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to prepare for workload installation?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task ConfigureScaleUnit(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                if (ScaleUnitContext.GetScaleUnitId() == "@@")
                    await new HubConfigurationManager().Configure();
                else
                    await new ScaleUnitConfigurationManager().Configure();
            }
        }
    }
}
