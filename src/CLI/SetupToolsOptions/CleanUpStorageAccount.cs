using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System;

namespace CLI.SetupToolsOptions
{
    internal class CleanUpStorageAccount : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(CleanUpScaleUnitStorageAccount);

            var screen = new CLIScreen(options, selectionHistory, "Please select the scale unit you would like to clean the storage account of:\n", "\nScale unit storage to clean up: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task CleanUpScaleUnitStorageAccount(int input, string selectionHistory)
        {
            using var context = ScaleUnitContext.CreateContext(GetScaleUnitId(input - 1));
            var storageAccountManager = new StorageAccountManager();
            await storageAccountManager.CleanStorageAccount();

            Console.WriteLine("Done");
        }
    }
}
