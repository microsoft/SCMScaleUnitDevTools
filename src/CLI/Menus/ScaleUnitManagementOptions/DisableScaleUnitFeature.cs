using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.ScaleUnitManagementOptions
{
    internal class DisableScaleUnitFeature : LeafMenu
    {
        public override string Description => "Disable scale unit feature and remove triggers";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to disable scale unit feature on?: ");
            await CLIController.ShowScreen(screen);
        }

        protected override async Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new DisableScaleUnitFeatureAction(scaleUnitId);
            await action.Execute();
        }
    }
}
