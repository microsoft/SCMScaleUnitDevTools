using System.Threading.Tasks;
using CLIFramework;
using CLI.Actions;
using System.Collections.Generic;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class DeleteWorkloads : ActionMenu
    {
        public override string Label => "Delete workloads from environment";

        protected override IAction Action => new DeleteWorkloadsAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to delete all workloads from?: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
