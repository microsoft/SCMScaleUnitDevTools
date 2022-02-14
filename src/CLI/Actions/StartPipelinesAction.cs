using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class StartPipelinesAction : ContextualAction
    {
        public StartPipelinesAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            var pipelineManager = new PipelineManager();
            await pipelineManager.StartWorkloadDataPipelines();
        }
    }
}
