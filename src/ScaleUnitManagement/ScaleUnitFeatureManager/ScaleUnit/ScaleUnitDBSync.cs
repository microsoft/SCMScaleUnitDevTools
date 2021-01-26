using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ScaleUnitDBSync : ScaleUnitStep
    {
        private readonly RunDBSync dbSync;

        public ScaleUnitDBSync()
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
            dbSync.Run();
        }
    }
}
