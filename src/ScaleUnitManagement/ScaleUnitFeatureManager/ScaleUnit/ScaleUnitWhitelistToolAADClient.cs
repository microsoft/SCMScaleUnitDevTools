using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public sealed class ScaleUnitWhitelistToolAADClient : IScaleUnitStep
    {
        private AddToolClientToSysAADClientTable AddToolClientToSysAADClientTable
            => new AddToolClientToSysAADClientTable();

        public string Label()
        {
            return this.AddToolClientToSysAADClientTable.Label();
        }

        public float Priority()
        {
            return this.AddToolClientToSysAADClientTable.Priority();
        }

        public void Run()
        {
            this.AddToolClientToSysAADClientTable.Run(Config.AxScaleUnitDbName());
        }
    }
}
