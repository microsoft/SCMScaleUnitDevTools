using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal class ConfigureEnvironment : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(ConfigureScaleUnit);
            var screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to prepare for workload installation?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task ConfigureScaleUnit(int input, string selectionHistory)
        {
            using var context = ScaleUnitContext.CreateContext(GetScaleUnitId(input - 1));
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubConfigurationManager().Configure();
            else
                await new ScaleUnitConfigurationManager().Configure();
        }
    }
}
