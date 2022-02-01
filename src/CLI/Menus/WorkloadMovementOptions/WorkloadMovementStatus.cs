using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadMovementOptions
{
    internal class WorkloadMovementStatus : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), ShowWorkloadMovementStatusForScaleUnit);

            var screen = new SingleSelectScreen(options, selectionHistory, "Show status of workload movement on:\n", "\nEnvironment?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task ShowWorkloadMovementStatusForScaleUnit(int input, string selectionHistory)
        {
            string scaleUnitId = GetSortedScaleUnits()[input - 1].ScaleUnitId;
            var action = new WorkloadsMovementStatusAction(scaleUnitId);
            await action.Execute();
        }
    }
}
