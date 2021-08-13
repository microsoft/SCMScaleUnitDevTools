using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal class InstallWorkloads
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = InstallWorkloadsForScaleUnit });
            }

            var screen = new CLIScreen(options, selectionHistory, "Install workloads on:\n", "\nEnvironment to install the workloads on?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task InstallWorkloadsForScaleUnit(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                if (ScaleUnitContext.GetScaleUnitId() == "@@")
                    await new HubWorkloadInstaller().Install();
                else
                    await new ScaleUnitWorkloadInstaller().Install();
            }
        }
    }
}
