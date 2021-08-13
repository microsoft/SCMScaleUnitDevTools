using System.Data.SqlClient;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class EnableNumberSequenceTableChangeTracking : IHubStep
    {
        public string Label()
        {
            return "Enable Number Sequence Table change tracking (temporary mitigation for #53)";
        }

        public float Priority()
        {
            return 3.6F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
USE {scaleUnit.AxDbName};
IF NOT EXISTS (SELECT TOP 1 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('NumberSequenceTable'))
	ALTER TABLE dbo.NumberSequenceTable ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON)
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
