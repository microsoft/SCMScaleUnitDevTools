using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    internal static class WorkloadInstallationMenu
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var installWorkloadsOption = new CLIOption() { Name = "Install workloads", Command = InstallWorkloads.Show };
            var workloadsInstallationStatusOption = new CLIOption() { Name = "Show workloads installation status", Command = WorkloadsInstallationStatus.Show };

            var options = new List<CLIOption>() { installWorkloadsOption, workloadsInstallationStatusOption };

            var screen = new CLIScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
