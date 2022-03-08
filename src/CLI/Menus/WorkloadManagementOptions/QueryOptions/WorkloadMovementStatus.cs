using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.QueryOptions
{
    internal class WorkloadMovementStatus : ActionMenu
    {
        public override string Label => "Show workload movement status";

        protected override IAction Action => new WorkloadsMovementStatusAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);

            var screen = new SingleSelectScreen(options, selectionHistory, "Show status of workload movement on:\n", "\nEnvironment?: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
