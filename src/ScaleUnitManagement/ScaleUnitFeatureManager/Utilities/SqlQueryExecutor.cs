using System.Data.SqlClient;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class SqlQueryExecutor
    {
        private readonly ScaleUnitInstance scaleUnit;

        public SqlQueryExecutor()
        {
            scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
        }

        public void Execute(string query)
        {
            string connectionString = $"Data Source=localhost;Initial Catalog={scaleUnit.AxDbName};Integrated Security=True;Enlist=True;Application Name=ScaleUnitDevTool";
            using (var conn = new SqlConnection(connectionString))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.CommandTimeout = 65535;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
