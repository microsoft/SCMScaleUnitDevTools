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
            var installationMenuOption = new CLIOption() { Name = "Show installation options", Command = InstallationMenu.Show };
            var movementMenuOption = new CLIOption() { Name = "Show movement options", Command = MovementMenu.Show };
            var deleteWorkloadsOption = new CLIOption() { Name = "Delete workloads from environment", Command = DeleteWorkloads.Show };
            var setupToolsOption = new CLIOption() { Name = "Setup tools", Command = SetupTools.Show };
            var options = new List<CLIOption>() { enableScaleUnitFeatureOption, installationMenuOption, movementMenuOption, deleteWorkloadsOption, setupToolsOption };

            var screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
