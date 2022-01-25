using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class ConfigureEnvironmentAction : ContextualAction
    {
        public ConfigureEnvironmentAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubConfigurationManager().Configure();
            else
                await new ScaleUnitConfigurationManager().Configure();
        }
    }
}
