using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.DatabaseManagementOptions
{
    internal class SyncDB : ActionMenu
    {
        public override string Label => "Sync DB";

        protected override IAction Action => new SyncDBAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
