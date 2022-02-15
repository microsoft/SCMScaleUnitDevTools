using System;
using System.Threading.Tasks;
using ScaleUnitManagement.DatabaseManager;

namespace CLI.Actions
{
    internal class CleanUpStorageAccountAction : ContextualAction
    {
        public CleanUpStorageAccountAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            try
            {
                var storageAccountManager = new StorageAccountManager();
                await storageAccountManager.CleanStorageAccount();
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occured while cleaning up storage: \n{ex}");
            }
        }
    }
}
