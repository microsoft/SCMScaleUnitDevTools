using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public sealed class AddToolClientToSysAADClientTable : CommonStep
    {
        public override string Label()
        {
            return "Add CLI tool App to SysAADClientTable";
        }

        public override float Priority()
        {
            return 6F;
        }

        public override void Run()
        {
            string sqlQuery = $@"
USE {Config.AxDbName()};

IF NOT EXISTS (SELECT TOP 1 1 FROM SysAADClientTable WHERE AADClientId = '{Config.AppId()}')
    INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{Config.AppId()}', 'Admin', 'Scale Unit Management Tool');
";

            string cmd = "Invoke-SqlCmd -Query " + CommandExecutor.Quotes + sqlQuery + CommandExecutor.Quotes + " -QueryTimeout 65535";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
