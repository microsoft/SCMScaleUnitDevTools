using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
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
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            string sqlQuery = $@"
USE {scaleUnit.AxDbName};

IF NOT EXISTS (SELECT TOP 1 1 FROM SysAADClientTable WHERE AADClientId = '{Config.AppId()}')
    INSERT INTO SysAADClientTable (AADClientId, UserId, Name) VALUES ('{Config.AppId()}', 'Admin', 'Scale Unit Management Tool');
";

            string cmd = "Invoke-SqlCmd -Query " + CommandExecutor.Quotes + sqlQuery + CommandExecutor.Quotes + " -QueryTimeout 65535";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);

            return Task.CompletedTask;
        }
    }
}
