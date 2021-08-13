using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    internal static class MovementMenu
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var moveWorkloadsOption = new CLIOption() { Name = "Move all workloads to the hub", Command = MoveWorkloads.MoveAllWorkloads };
            var workloadMovementStatusOption = new CLIOption() { Name = "Show workload movement status", Command = WorkloadMovementStatus.Show };

            var options = new List<CLIOption>() { moveWorkloadsOption, workloadMovementStatusOption };

            var screen = new CLIScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
