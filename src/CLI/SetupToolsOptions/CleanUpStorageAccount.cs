using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System;
using System.Collections.Generic;

namespace CLI.SetupToolsOptions
{
    internal class CleanUpStorageAccount : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), CleanUpScaleUnitStorageAccount);

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the scale unit you would like to clean the storage account of:\n", "\nScale unit storage to clean up: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task CleanUpScaleUnitStorageAccount(int input, string selectionHistory)
        {
            var storageAccountManager = new StorageAccountManager();
            await storageAccountManager.CleanStorageAccount();

            Console.WriteLine("Done");
        }
    }
}
