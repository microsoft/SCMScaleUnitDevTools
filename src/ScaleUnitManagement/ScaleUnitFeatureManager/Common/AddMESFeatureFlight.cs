using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public sealed class AddMESFeatureFlight : ICommonStep
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

        public void Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
USE {scaleUnit.AxDbName};

-- insert row under Hub scale unit id to avoid merge conflicts later, as the table is synced Hub->Scale Unit
EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '@@';

IF NOT EXISTS (SELECT TOP 1 1 FROM SysFlighting WHERE FlightName = '{MESFlightName}')
    INSERT INTO SysFlighting (FlightName, Enabled, FlightServiceId) VALUES ('{MESFlightName}', 1, 12719367);
";

            string cmd = "Invoke-SqlCmd -Query " + CommandExecutor.Quotes + sqlQuery + CommandExecutor.Quotes + " -QueryTimeout 65535";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
