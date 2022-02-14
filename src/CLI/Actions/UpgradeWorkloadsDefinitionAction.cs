using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class UpgradeWorkloadsDefinitionAction : ContextualAction
    {
        public UpgradeWorkloadsDefinitionAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            var workloadDefinitionManager = new WorkloadDefinitionManager();
            await workloadDefinitionManager.UpgradeWorkloadsDefinition();
        }
    }
}
