using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public sealed class StartHubServices : IHubStep
    {
        private StartServices StartServices => new StartServices();

        public string Label()
        {
            return this.StartServices.Label();
        }

        public float Priority()
        {
            return this.StartServices.Priority();
        }

        public void Run()
        {
            this.StartServices.Run(
                batchName: Config.HubBatchName,
                appPoolName: Config.HubAppPoolName,
                siteName: Config.HubAppPoolName);
        }
    }
}
