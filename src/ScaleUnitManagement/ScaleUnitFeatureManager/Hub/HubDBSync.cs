using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class HubDBSync : HubStep
    {
        RunDBSync dbSync;

        public HubDBSync()
        {
            dbSync = new RunDBSync();
        }

        public override string Label()
        {
            return dbSync.Label();
        }

        public override float Priority()
        {
            return dbSync.Priority();
        }

        public override void Run()
        {
            dbSync.Run("@@");
        }
    }
}
