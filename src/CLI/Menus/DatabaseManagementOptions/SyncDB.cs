using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.DatabaseManagementOptions
{
    internal class SyncDB : LeafMenu
    {
        public override string Description => "Sync DB";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIController.ShowScreen(screen);
        }

        protected override async Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new SyncDBAction(scaleUnitId);
            await action.Execute();
        }
    }
}
