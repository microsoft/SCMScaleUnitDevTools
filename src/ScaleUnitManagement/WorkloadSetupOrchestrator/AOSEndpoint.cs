using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public abstract class AOSEndpoint
    {
        protected IAOSClient aosClient = null;
        protected readonly ScaleUnitInstance scaleUnit;

        public AOSEndpoint()
        {
            this.scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
        }

        protected async Task EnsureClientInitialized()
        {
            if (aosClient is null)
            {
                SetClient(await AOSClient.Construct(scaleUnit));
            }
        }

        internal void SetClient(IAOSClient aosClient)
        {
            this.aosClient = aosClient;
        }
    }
}
