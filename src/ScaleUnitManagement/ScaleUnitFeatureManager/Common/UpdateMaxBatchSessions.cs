using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class UpdateMaxBatchSessions : ICommonStep
    {
        public string Label()
        {
            return "Update max batch sessions";
        }

        public float Priority()
        {
            return 3.5F;
        }

        public Task Run()
        {
            int maxBatchSessions = 16;
            string query = $"UPDATE t1 " +
                $"SET t1.MAXBATCHSESSIONS = {maxBatchSessions} " +
                $"FROM BATCHSERVERCONFIG t1 " +
                $"WHERE t1.MAXBATCHSESSIONS < {maxBatchSessions}";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(query);

            return Task.CompletedTask;
        }
    }
}
