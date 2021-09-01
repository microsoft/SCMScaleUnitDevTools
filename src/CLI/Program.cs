using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    public class Program
    {
        private static bool repeat = true;

        public static async Task Main(string[] args)
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            var options = new List<CLIOption>() {
                Option("Initialize the hybrid topology", EnableScaleUnitFeature.SelectScaleUnit),
                Option("Prepare environments for workload installation", ConfigureEnvironment.Show),
                Option("Show workload installation options", WorkloadInstallationMenu.Show),
                Option("Show workload movement options", WorkloadMovementMenu.Show),
                Option("Delete workloads from environment", DeleteWorkloads.Show),
                Option("Setup tools", SetupTools.Show),
                Option("Upgrade workloads definition", UpgradeWorkloadsDefinition.UpgradeAllWorkloadDefinitions),
                Option("Show workload data pipeline management options", ManageWorkloadDataPipeline.Show),
                Option("Exit", ExitTool),
            };

            while (repeat)
            {
                var screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
                await CLIMenu.ShowScreen(screen);
                CLIMenu.PressToContinue();
            }
        }

        private static Task ExitTool(int input, string selectionHistory)
        {
            repeat = false;
            return Task.CompletedTask;
        }

        static CLIOption Option(string name, Func<int, string, Task> command)
        {
            return new CLIOption { Name = name, Command = command };
        }
    }
}
