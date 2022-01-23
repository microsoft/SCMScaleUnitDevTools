using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.WorkloadMovementOptions
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
            var workloadMover = new WorkloadMover();
            await workloadMover.ShowMovementStatus();
        }
    }
}
