using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class InstallWorkloadsAction : ContextualAction
    {
        public InstallWorkloadsAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubWorkloadInstaller().Install();
            else
                await new ScaleUnitWorkloadInstaller().Install();
        }
    }
}
