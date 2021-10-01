using System.Threading.Tasks;
using CLIFramework;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System.Collections.Generic;

namespace CLI.SetupToolsOptions
{
    internal class SyncDB : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), RunSyncDB);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIController.ShowScreen(screen);
        }

        private Task RunSyncDB(int input, string selectionHistory)
        {
            try
            {
                new RunDBSync().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to run DbSync:\n{ex}");
            }
            return Task.CompletedTask;
        }
    }
}
