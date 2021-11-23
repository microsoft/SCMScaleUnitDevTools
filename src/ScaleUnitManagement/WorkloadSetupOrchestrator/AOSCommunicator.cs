using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public abstract class AOSCommunicator
    {
        protected IAOSClient scaleUnitAosClient = null;
        protected IAOSClient hubAosClient = null;
        protected readonly ScaleUnitInstance scaleUnit;

        public AOSCommunicator()
        {
            scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
        }

        protected async Task<IAOSClient> GetScaleUnitAosClient()
        {
            if (scaleUnitAosClient is null)
            {
                ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
                await ReliableRun.Execute(async () => SetScaleUnitAosClient(await AOSClient.Construct(scaleUnit)), "Connecting to AOS");
            }
            return scaleUnitAosClient;
        }

        protected async Task<IAOSClient> GetHubAosClient()
        {
            if (hubAosClient is null)
            {
                ScaleUnitInstance hub = Config.HubScaleUnit();
                await ReliableRun.Execute(async () => SetHubAosClient(await AOSClient.Construct(hub)), "Connecting to AOS");
            }
            return hubAosClient;
        }

        internal void SetScaleUnitAosClient(IAOSClient aosClient)
        {
            this.scaleUnitAosClient = aosClient;
        }

        internal void SetHubAosClient(IAOSClient aosClient)
        {
            this.hubAosClient = aosClient;
        }
    }
}
