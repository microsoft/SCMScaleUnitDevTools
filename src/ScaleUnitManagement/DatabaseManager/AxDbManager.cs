using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.DatabaseManager
{
    public class AxDbManager
    {
        public static bool UserExists(string userName)
        {
            string query = $"select count(name) from USERINFO where name = '{userName}'";

            var sqlQueryExecutor = new SqlQueryExecutor();
            return sqlQueryExecutor.ExecuteBooleanQuery(query);
        }
    }
}
