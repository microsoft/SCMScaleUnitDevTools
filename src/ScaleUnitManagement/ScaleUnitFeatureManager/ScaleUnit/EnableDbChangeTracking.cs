using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class EnableDbChangeTracking : ScaleUnitStep
    {
        public override string Label()
        {
            return "Enable DB change tracking";
        }

        public override float Priority()
        {
            return 3.5F;
        }

        public override void Run()
        {
            string sqlQuery = $"USE master; IF NOT EXISTS (SELECT * FROM sys.change_tracking_databases WHERE database_id=DB_ID('{Config.AxDbName()}')) ALTER DATABASE {Config.AxDbName()} SET CHANGE_TRACKING = ON(CHANGE_RETENTION = 2 DAYS, AUTO_CLEANUP = ON)";
            string cmd = "Invoke-Sqlcmd -Query " + CommandExecutor.Quotes + sqlQuery + CommandExecutor.Quotes + " -QueryTimeout 65535";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
