using System.Data.SqlClient;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ClearProblematicTables : IScaleUnitStep
    {
        public string Label()
        {
            return "Truncate potentially problematic tables";
        }

        public float Priority()
        {
            return 2.5F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
            USE {scaleUnit.AxDbName};
            EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '';

            DELETE FROM SysFeatureStateV0;
            DELETE FROM FeatureManagementState;
            DELETE FROM FeatureManagementMetadata;
            DELETE FROM SysFlighting;

            TRUNCATE TABLE NumberSequenceScope;

            EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '@A';
            ";

            string connectionString = $"Data Source=localhost;Initial Catalog={scaleUnit.AxDbName};Integrated Security=True;Enlist=True;Application Name=ScaleUnitDevTool";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, conn))
                {
                    conn.Open();
                    cmd.CommandTimeout = 65535;
                    cmd.ExecuteNonQuery();
                }
            }

            return Task.CompletedTask;
        }
    }
}
