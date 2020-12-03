using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class WorkloadsInstallationStatus
    {
        public static async Task Show(int input, string selectionHistory)
        {

            CLIOption WorkloadsInstallationStatusOnHubOption = new CLIOption() { Name = "Hub", Command = new HubWorkloadInstaller().InstallationStatus };
            CLIOption WorkloadsInstallationStatusOnScaleUnitOption = new CLIOption() { Name = "Scale unit", Command = new ScaleUnitWorkloadInstaller().InstallationStatus };

            var options = new List<CLIOption>() { WorkloadsInstallationStatusOnHubOption, WorkloadsInstallationStatusOnScaleUnitOption };

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Show status of workloads installation on:\n", "\nEnvironment?: ");

            await CLIMenu.ShowScreen(screen);
        }
    }
}
