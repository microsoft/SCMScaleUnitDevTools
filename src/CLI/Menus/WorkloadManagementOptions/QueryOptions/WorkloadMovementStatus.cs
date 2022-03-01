using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.QueryOptions
{
    internal class WorkloadMovementStatus : LeafMenu
    {
        public override string Description => "Show workload movement status";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);

            var screen = new SingleSelectScreen(options, selectionHistory, "Show status of workload movement on:\n", "\nEnvironment?: ");
            await CLIController.ShowScreen(screen);
        }

        protected async override Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new WorkloadsMovementStatusAction(scaleUnitId);
            await action.Execute();
        }
    }
}
