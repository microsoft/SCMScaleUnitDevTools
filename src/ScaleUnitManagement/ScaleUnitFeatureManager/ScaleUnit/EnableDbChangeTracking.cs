using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class EnableDbChangeTracking : IScaleUnitStep
    {
        public string Label()
        {
            return "Enable DB change tracking";
        }

        public float Priority()
        {
            return 3.5F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $"USE master; IF NOT EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id=DB_ID('{scaleUnit.AxDbName}')) ALTER DATABASE {scaleUnit.AxDbName} SET CHANGE_TRACKING = ON(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(sqlQuery);

            return Task.CompletedTask;
        }
    }
}
