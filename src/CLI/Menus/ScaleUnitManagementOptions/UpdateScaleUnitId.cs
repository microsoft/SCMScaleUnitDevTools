using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using CLI.Actions;

namespace CLI.Menus.ScaleUnitManagementOptions
{
    internal class UpdateScaleUnitId : ActionMenu
    {
        private List<ScaleUnitInstance> sortedNonHubScaleUnits;

        public override string Label => "Update scale unit id";

        protected override IAction Action => new UpdateScaleUnitIdAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            sortedNonHubScaleUnits = Config.NonHubScaleUnitInstances();
            sortedNonHubScaleUnits.Sort();
            List<CLIOption> options = SelectScaleUnitOptions(sortedNonHubScaleUnits, UpdateId);

            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to update the scale unit id of?: ");
            await CLIController.ShowScreen(screen);
        }

        protected async Task UpdateId(int input, string selectionHistory)
        {
            scaleUnitId = sortedNonHubScaleUnits[input - 1].ScaleUnitId;
            await Action.Execute();
        }
    }
}
