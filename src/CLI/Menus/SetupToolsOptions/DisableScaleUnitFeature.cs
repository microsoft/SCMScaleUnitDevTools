using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.SetupToolsOptions
{
    internal class DisableScaleUnitFeature : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), RunDisableScaleUnitFeature);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to disable scale unit feature on?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task RunDisableScaleUnitFeature(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new DisableScaleUnitFeatureAction(scaleUnitId);
            await action.Execute();
        }
    }
}
