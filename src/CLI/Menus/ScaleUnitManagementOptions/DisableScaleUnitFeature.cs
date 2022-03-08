using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.ScaleUnitManagementOptions
{
    internal class DisableScaleUnitFeature : ActionMenu
    {
        public override string Label => "Disable scale unit feature and remove triggers";

        protected override IAction Action => new DisableScaleUnitFeatureAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to disable scale unit feature on?: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
