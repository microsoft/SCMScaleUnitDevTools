using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.WorkloadInstallationOptions;
using CLIFramework;

namespace CLI
{
    internal class WorkloadInstallationMenu : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>()
            {
                Option("Install workloads", new InstallWorkloads().Show),
                Option("Show workloads installation status", new WorkloadsInstallationStatus().Show),
            };

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
