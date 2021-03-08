using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class HubDBSync : IHubStep
    {
        private readonly RunDBSync dbSync;

        public HubDBSync()
        {
            dbSync = new RunDBSync();
        }

        public string Label()
        {
            return dbSync.Label();
        }

        public float Priority()
        {
            return dbSync.Priority();
        }

        public Task Run()
        {
            dbSync.Run();

            return Task.CompletedTask;
        }
    }
}
