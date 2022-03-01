using System.Threading.Tasks;
using CLIFramework;
using CLI.Actions;
using System.Collections.Generic;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class DeleteWorkloads : LeafMenu
    {
        public override string Description => "Delete workloads from environment";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to delete all workloads from?: ");
            await CLIController.ShowScreen(screen);
        }
        protected async override Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new DeleteWorkloadsAction(scaleUnitId);
            await action.Execute();
        }
    }
}
