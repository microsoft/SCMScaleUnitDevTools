using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class EnableNumberSequenceTableChangeTracking : IHubStep
    {
        public string Label()
        {
            return "Enable Number Sequence Table change tracking (temporary mitigation for #53)";
        }

        public float Priority()
        {
            return 3.6F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string sqlQuery = $@"
            USE {scaleUnit.AxDbName};
            IF NOT EXISTS (SELECT TOP 1 1 FROM sys.change_tracking_tables WHERE object_id = OBJECT_ID('NumberSequenceTable'))
	            ALTER TABLE dbo.NumberSequenceTable ENABLE CHANGE_TRACKING WITH (TRACK_COLUMNS_UPDATED = ON)
            ";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(sqlQuery);

            return Task.CompletedTask;
        }
    }
}
