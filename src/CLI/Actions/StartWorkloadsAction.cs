using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class StartWorkloadsAction : ContextualAction
    {
        public StartWorkloadsAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            var pipelineManager = new PipelineManager();
            await pipelineManager.StartWorkloadDataPipelines();
        }
    }
}
