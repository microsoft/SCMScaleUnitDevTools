using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.DatabaseManagementOptions
{
    internal class CleanUpStorageAccount : ActionMenu
    {
        public override string Label => "Clean up Azure storage account for a scale unit";

        protected override IAction Action => new CleanUpStorageAccountAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the scale unit you would like to clean the storage account of:\n", "\nScale unit storage to clean up: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
