using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public sealed class AddMESFeatureFlight : IHubStep
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

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
            USE {scaleUnit.AxDbName};
            IF NOT EXISTS (SELECT TOP 1 1 FROM SysFlighting WHERE FlightName = '{MESFlightName}')
                INSERT INTO SysFlighting (FlightName, Enabled, FlightServiceId) VALUES ('{MESFlightName}', 1, 12719367);
            ";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(sqlQuery);

            return Task.CompletedTask;
        }
    }
}
