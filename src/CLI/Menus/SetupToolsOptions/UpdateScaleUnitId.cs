using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using CLI.Actions;

namespace CLI.Menus.SetupToolsOptions
{
    internal class UpdateScaleUnitId : DevToolMenu
    {
        private List<ScaleUnitInstance> sortedNonHubScaleUnits;

        public override async Task Show(int input, string selectionHistory)
        {
            sortedNonHubScaleUnits = Config.NonHubScaleUnitInstances();
            sortedNonHubScaleUnits.Sort();
            List<CLIOption> options = SelectScaleUnitOptions(sortedNonHubScaleUnits, RunUpdateScaleunitId);

            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to update the scale unit id of?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task RunUpdateScaleunitId(int input, string selectionHistory)
        {
            string scaleUnitId = sortedNonHubScaleUnits[input - 1].ScaleUnitId;
            var action = new UpdateScaleUnitIdAction(scaleUnitId);
            await action.Execute();
        }
    }
}
