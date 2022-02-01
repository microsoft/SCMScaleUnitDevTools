using System.Threading.Tasks;
using CLIFramework;
using CLI.Actions;
using System.Collections.Generic;

namespace CLI.Menus
{
    internal class DeleteWorkloads : DevToolMenu
    {

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), DeleteWorkloadsFromScaleUnit);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to delete all workloads from?: ");
            await CLIController.ShowScreen(screen);
        }

        public async Task DeleteWorkloadsFromScaleUnit(int input, string selectionHistory)
        {
            string scaleUnitId = GetSortedScaleUnits()[input - 1].ScaleUnitId;
            var action = new DeleteWorkloadsAction(scaleUnitId);
            await action.Execute();
        }
    }
}
