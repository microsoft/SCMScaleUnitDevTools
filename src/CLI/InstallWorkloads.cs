using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class InstallWorkloads
    {
        public static async Task Show(int input, string selectionHistory)
        {

            CLIOption installWorkloadsOnHubOption = new CLIOption() { Name = "Hub", Command = new HubWorkloadInstaller().Install };
            CLIOption installWorkloadsOnScaleUnitOption = new CLIOption() { Name = "Scale unit", Command = new ScaleUnitWorkloadInstaller().Install };

            var options = new List<CLIOption>() { installWorkloadsOnHubOption, installWorkloadsOnScaleUnitOption };

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Install workloads on:\n", "\nEnvironment to install the workloads on?: ");

            await CLIMenu.ShowScreen(screen);
        }
    }
}
