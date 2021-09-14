using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;
using CLIFramework;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    internal class DeleteWorkloads : DevToolMenu
    {

        public override async Task Show(int input, string selectionHistory)
        {
            System.Collections.Generic.List<CLIOption> options = SelectScaleUnitOptions(DeleteWorkloadsFromScaleUnit);
            var screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to delete all workloads from?: ");
            await CLIController.ShowScreen(screen);
        }

        public async Task DeleteWorkloadsFromScaleUnit(int input, string selectionHistory)
        {
            using var context = ScaleUnitContext.CreateContext(GetScaleUnitId(input - 1));
            var workloadDeleter = new WorkloadDeleter();
            await workloadDeleter.DeleteWorkloadsFromScaleUnit();

            Console.WriteLine("Done.");
        }
    }
}
