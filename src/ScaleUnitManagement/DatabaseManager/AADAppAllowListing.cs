using System;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.DatabaseManager
{
    public class AADAppAllowListing
    {
        public void UpdateAADAppClientTable(string dbName, string userName, string appName, string appId)
        {
            string sqlQuery = $@"
            USE {dbName};

            IF NOT EXISTS (SELECT TOP 1 1 FROM USERINFO WHERE ID = '{userName}')
                THROW 51000, 'No user exists named {userName}', 1
            ELSE
                IF NOT EXISTS (SELECT TOP 1 1 FROM SysAADClientTable WHERE AADClientId = '{appId}')
                    INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{appId}', '{userName}', '{appName}');
            ";

            var sqlQueryExecutor = new SqlQueryExecutor();

            try
            {
                sqlQueryExecutor.Execute(sqlQuery);
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains($"No user exists named {userName}"))
                {
                    Console.WriteLine($"\nUser \"{userName}\" does not exist, run Deployment.Setup.exe fullall sync to create it\n");
                }
                throw;
            }


        }
    }
}
