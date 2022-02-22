using System;
using System.Data.SqlClient;

namespace ScaleUnitManagement.Utilities
{
    public class SqlQueryExecutor
    {
        private readonly string dbName;
        private readonly ScaleUnitInstance scaleUnit;

        public SqlQueryExecutor()
        {
            scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            dbName = scaleUnit.AxDbName;
        }

        public SqlQueryExecutor(string dbName)
        {
            this.dbName = dbName;
        }

        private string ConnectionString()
        {
            return $"Data Source=localhost;Initial Catalog={dbName};Integrated Security=True;Enlist=True;Application Name=ScaleUnitDevTool";
        }

        public void Execute(string query)
        {
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.CommandTimeout = 65535;
                cmd.ExecuteNonQuery();
            }
        }

        public bool ExecuteBooleanQuery(string query)
        {
            using (var conn = new SqlConnection(ConnectionString()))
            using (var cmd = new SqlCommand(query, conn))
            {
                conn.Open();
                cmd.CommandTimeout = 65535;
                SqlDataReader reader = cmd.ExecuteReader();
                try
                {
                    reader.Read();
                    int value = reader.GetInt32(0);
                    return value != 0;
                }
                catch
                {
                    Console.WriteLine("sql query did not return a boolean result");
                    throw;
                }
            }
        }
    }
}
