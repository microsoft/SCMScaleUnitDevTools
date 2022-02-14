using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLI.Menus.WorkloadMovementOptions;
using CLIFramework;

namespace CLI.Menus
{
    internal class WorkloadMovementMenu : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>()
            {
                Option("Move all workloads to the hub", MoveAllWorkloads),
                Option("Show workload movement status", new WorkloadMovementStatus().Show),
                Option("Perform emergency transition from scale unit to hub", new PerformEmergencyTransitionToHub().Show)
            };

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }

        public async Task MoveAllWorkloads(int input, string selectionHistory)
        {
            await new MoveAllWorkloadsAction().Execute();
        }

    }
}
