using System.Threading.Tasks;
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
            // before starting services again
            return 3.9F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
USE {scaleUnit.AxDbName};

IF NOT EXISTS (SELECT TOP 1 1 FROM SysFlighting WHERE FlightName = '{MESFlightName}')
    INSERT INTO SysFlighting (FlightName, Enabled, FlightServiceId) VALUES ('{MESFlightName}', 1, 12719367);
";

            string cmd = "Invoke-SqlCmd -Query " + CommandExecutor.Quotes + sqlQuery + CommandExecutor.Quotes + " -QueryTimeout 65535";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);

            return Task.CompletedTask;
        }
    }
}
