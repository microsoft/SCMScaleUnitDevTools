using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            var enableScaleUnitFeatureOption = new CLIOption() { Name = "Initialize the hybrid topology", Command = EnableScaleUnitFeature.SelectScaleUnit };
            var configureEnvironmentOption = new CLIOption() { Name = "Prepare environments for workload installation", Command = ConfigureEnvironment.Show };
            var installationMenuOption = new CLIOption() { Name = "Show workload installation options", Command = WorkloadInstallationMenu.Show };
            var movementMenuOption = new CLIOption() { Name = "Show workload movement options", Command = WorkloadMovementMenu.Show };
            var deleteWorkloadsOption = new CLIOption() { Name = "Delete workloads from environment", Command = DeleteWorkloads.Show };
            var setupToolsOption = new CLIOption() { Name = "Setup tools", Command = SetupTools.Show };
            var options = new List<CLIOption>() { enableScaleUnitFeatureOption, configureEnvironmentOption, installationMenuOption, movementMenuOption, deleteWorkloadsOption, setupToolsOption };

            var screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
