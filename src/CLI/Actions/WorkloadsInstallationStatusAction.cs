using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class WorkloadsInstallationStatusAction : ContextualAction
    {
        public WorkloadsInstallationStatusAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubWorkloadInstaller().InstallationStatus();
            else
                await new ScaleUnitWorkloadInstaller().InstallationStatus();
        }
    }
}
