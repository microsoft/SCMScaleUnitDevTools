using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ScaleUnitDBSync : IScaleUnitStep
    {
        private readonly RunDBSync dbSync;

        public ScaleUnitDBSync()
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
