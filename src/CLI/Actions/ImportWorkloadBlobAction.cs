using System;
using System.Threading.Tasks;
using ScaleUnitManagement.DatabaseManager;

namespace CLI.Actions
{
    internal class ImportWorkloadBlobAction : ContextualAction
    {
        private readonly string sasToken;

        public ImportWorkloadBlobAction(string scaleUnitId, string sasToken) : base(scaleUnitId)
        {
            this.sasToken = sasToken;
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            var storageAccountManager = new StorageAccountManager();
            try
            {
                await storageAccountManager.ImportWorkloadsBlob(sasToken);
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occured while importing blobs: {ex.Message}");
            }
        }
    }
}
