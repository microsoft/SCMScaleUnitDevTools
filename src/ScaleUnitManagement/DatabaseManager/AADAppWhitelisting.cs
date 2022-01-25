using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.DatabaseManager
{
    public class AADAppWhitelisting
    {
        public void UpdateAADAppClientTable(string dbName, string userName, string appName, string appId)
        {
            string sqlQuery = $@"
            USE {dbName};

            IF NOT EXISTS (SELECT TOP 1 1 FROM SysAADClientTable WHERE AADClientId = '{appId}')
                BEGIN
                    IF NOT EXISTS (SELECT TOP 1 1 FROM SysAADClientTable WHERE NAME = '{appName}')
                        BEGIN
                            IF EXISTS (SELECT TOP 1 1 FROM USERINFO WHERE ID = '{userName}')
                                INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{appId}', '{userName}', '{appName}');
                            ELSE
                                INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{appId}', 'Admin', '{appName}');
                        END
                    ELSE
                        BEGIN
                            IF EXISTS (SELECT TOP 1 1 FROM USERINFO WHERE ID = '{userName}')
                                UPDATE SysAADClientTable SET 
                                    AADClientId = '{appId}',
                                    UserId = '{userName}'
                                WHERE NAME = '{appName}'
                            ELSE
                                UPDATE SysAADClientTable SET 
                                    AADClientId = '{appId}',
                                    UserId = 'Admin'
                                WHERE NAME = '{appName}'
                        END
                 END
            ";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(sqlQuery);
        }
    }
}
