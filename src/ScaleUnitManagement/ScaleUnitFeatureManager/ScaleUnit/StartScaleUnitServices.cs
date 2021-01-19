using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public sealed class StartScaleUnitServices : IScaleUnitStep
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
                batchName: Config.ScaleUnitBatchName,
                appPoolName: Config.ScaleUnitAppPoolName,
                siteName: Config.ScaleUnitAppPoolName);
        }
    }
}
