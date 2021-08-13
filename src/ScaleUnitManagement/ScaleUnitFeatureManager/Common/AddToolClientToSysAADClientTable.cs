using System.Data.SqlClient;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public sealed class AddToolClientToSysAADClientTable : ICommonStep
    {
        public string Label()
        {
            return "Add CLI tool App to SysAADClientTable";
        }

        public float Priority()
        {
            return 6F;
        }

        public Task Run()
        {
            const string ScaleUnitManagementUserName = "ScaleUnitManagement";

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
            USE {scaleUnit.AxDbName};

            IF NOT EXISTS (SELECT TOP 1 1 FROM SysAADClientTable WHERE AADClientId = '{scaleUnit.AuthConfiguration.AppId}')
            BEGIN
                IF EXISTS (SELECT TOP 1 1 FROM USERINFO WHERE ID = '{ScaleUnitManagementUserName}')
                    INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{scaleUnit.AuthConfiguration.AppId}', '{ScaleUnitManagementUserName}', 'Scale Unit Management Tool');
                ELSE
                    INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{scaleUnit.AuthConfiguration.AppId}', 'Admin', 'Scale Unit Management Tool');
            END
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
