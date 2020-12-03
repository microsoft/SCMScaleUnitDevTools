using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class ConfigureEnvironment
    {
        public static async Task Show(int input, string selectionHistory)
        {
            CLIOption configureHubOption = new CLIOption() { Name = "Hub", Command = new HubConfigurationManager().Configure };
            CLIOption configureScaleUnitOption = new CLIOption() { Name = "Scale unit", Command = new ScaleUnitConfigurationManager().Configure };

            var options = new List<CLIOption>() { configureHubOption, configureScaleUnitOption };

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Type of environment to configure:\n", "\nWould you like to configure the environment as the hub or a scale unit?: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
