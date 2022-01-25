using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class WorkloadsMovementStatusAction : ContextualAction
    {
        public WorkloadsMovementStatusAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            var workloadMover = new WorkloadMover();
            await workloadMover.ShowMovementStatus();
        }
    }
}
