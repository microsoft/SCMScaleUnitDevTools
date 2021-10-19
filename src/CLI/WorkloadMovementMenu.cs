using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using CLI.WorkloadMovementOptions;

namespace CLI
{
    internal class WorkloadMovementMenu : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>()
            {
                Option("Move all workloads to the hub", new MoveWorkloads().MoveAllWorkloads),
                Option("Show workload movement status", new WorkloadMovementStatus().Show),
            };

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
