using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class WorkloadsInstallationStatus
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = ShowWorkloadInstallationStatusForScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Show status of workloads installation on:\n", "\nEnvironment?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task ShowWorkloadInstallationStatusForScaleUnit(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                if (ScaleUnitContext.GetScaleUnitId() == "@@")
                    await new HubWorkloadInstaller().InstallationStatus();
                else
                    await new ScaleUnitWorkloadInstaller().InstallationStatus();
            }
        }
    }
}
