using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.DatabaseManagementOptions
{
    internal class CleanUpStorageAccount : LeafMenu
    {
        public override string Description => "Clean up Azure storage account for a scale unit";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the scale unit you would like to clean the storage account of:\n", "\nScale unit storage to clean up: ");
            await CLIController.ShowScreen(screen);
        }

        protected override async Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new CleanUpStorageAccountAction(scaleUnitId);
            await action.Execute();
        }
    }
}
