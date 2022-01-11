using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class SetAutoClose : IScaleUnitStep
    {
        public string Label()
        {
            return "Set DB auto-close to false";
        }

        public float Priority()
        {
            return 3.5F;
        }

        public Task Run()
        {
            string axDbName = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId()).AxDbName;
            string query = $"ALTER DATABASE {axDbName} SET AUTO_CLOSE OFF";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(query);

            return Task.CompletedTask;
        }
    }
}
