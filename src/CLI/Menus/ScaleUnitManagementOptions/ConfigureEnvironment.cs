using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.ScaleUnitManagementOptions
{
    internal class ConfigureEnvironment : LeafMenu
    {
        public override string Description => "Prepare environments for workload installation";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to prepare for workload installation?: ");
            await CLIController.ShowScreen(screen);
        }

        protected override async Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new ConfigureEnvironmentAction(scaleUnitId);
            await action.Execute();
        }
    }
}
