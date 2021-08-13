using System.Data.SqlClient;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public sealed class AddMESFeatureFlight : IHubStep
    {
        private const string MESFlightName = "SysWorkloadTypeMESFeatureToggle";

        public string Label()
        {
            return "Add MES feature flight";
        }

        public float Priority()
        {
            // after DbSync (we need the triggers and SYSSCALEUNITID columns),
            // before starting services again,
            return 3.9F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
            USE {scaleUnit.AxDbName};
            IF NOT EXISTS (SELECT TOP 1 1 FROM SysFlighting WHERE FlightName = '{MESFlightName}')
                INSERT INTO SysFlighting (FlightName, Enabled, FlightServiceId) VALUES ('{MESFlightName}', 1, 12719367);
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
