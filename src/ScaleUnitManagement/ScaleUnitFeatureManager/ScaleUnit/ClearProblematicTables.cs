using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ClearProblematicTables
    // TODO : IScaleUnitStep
    {
        public string Label()
        {
            return "Truncate potentially problematic tables";
        }

        public float Priority()
        {
            return 2.5F;
        }

        public void Run()
        {
            string sqlQuery = $@"
USE {Config.AxScaleUnitDbName()};
EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '';

DELETE FROM SysFeatureStateV0;
DELETE FROM FeatureManagementState;
DELETE FROM FeatureManagementMetadata;
DELETE FROM SysFlighting;

TRUNCATE TABLE NumberSequenceScope;

EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '@A';
";

            string cmd = "Invoke-Sqlcmd -Query " + CommandExecutor.Quotes + sqlQuery + CommandExecutor.Quotes + " -QueryTimeout 65535";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
