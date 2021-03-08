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
            var installWorkloadsOption = new CLIOption() { Name = "Install workloads", Command = InstallWorkloads.Show };
            var workloadsInstallationStatusOption = new CLIOption() { Name = "Show workloads installation status", Command = WorkloadsInstallationStatus.Show };

            var options = new List<CLIOption>() { enableScaleUnitFeatureOption, configureEnvironmentOption, installWorkloadsOption, workloadsInstallationStatusOption };

            var screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
