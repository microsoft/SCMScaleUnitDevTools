using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CLIOption enableScaleUnitFeatureOption = new CLIOption() { Name = "Enable the environment as the hub or a scale unit", Command = EnableScaleUnitFeature.SelectScaleUnit };
            CLIOption configureEnvironmentOption = new CLIOption() { Name = "Configure the environment", Command = ConfigureEnvironment.Show };
            CLIOption installWorkloadsOption = new CLIOption() { Name = "Install workloads on environment", Command = InstallWorkloads.Show };
            CLIOption workloadsInstallationStatusOption = new CLIOption() { Name = "Show workloads installation status", Command = WorkloadsInstallationStatus.Show };

            var options = new List<CLIOption>() { enableScaleUnitFeatureOption, configureEnvironmentOption, installWorkloadsOption, workloadsInstallationStatusOption };

            CLIScreen screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
