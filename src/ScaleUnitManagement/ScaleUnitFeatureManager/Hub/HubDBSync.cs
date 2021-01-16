using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class HubDBSync : IHubStep
    {
        private readonly RunDBSync dbSync;

        public HubDBSync()
        {
            dbSync = new RunDBSync();
        }

        public  string Label()
        {
            return dbSync.Label();
        }

        public  float Priority()
        {
            return dbSync.Priority();
        }

        public  void Run()
        {
            dbSync.Run("@@", Config.AxDbName());
        }
    }
}
