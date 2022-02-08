using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.SetupToolsOptions
{
    internal class SyncDB : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), RunSyncDB);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task RunSyncDB(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new SyncDBAction(scaleUnitId);
            await action.Execute();
        }
    }
}
