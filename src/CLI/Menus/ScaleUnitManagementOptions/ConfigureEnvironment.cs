using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.ScaleUnitManagementOptions
{
    internal class ConfigureEnvironment : ActionMenu
    {
        public override string Label => "Prepare environments for workload installation";

        protected override IAction Action => new ConfigureEnvironmentAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to prepare for workload installation?: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
