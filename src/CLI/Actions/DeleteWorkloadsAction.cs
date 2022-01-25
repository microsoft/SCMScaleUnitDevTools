using System;
using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class DeleteWorkloadsAction : ContextualAction
    {
        public DeleteWorkloadsAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            var workloadDeleter = new WorkloadDeleter();
            await workloadDeleter.DeleteWorkloadsFromScaleUnit();

            Console.WriteLine("Done.");
        }
    }
}
