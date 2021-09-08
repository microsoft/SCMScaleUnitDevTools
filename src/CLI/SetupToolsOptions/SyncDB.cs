using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;

namespace CLI.SetupToolsOptions
{
    internal class SyncDB : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = SelectScaleUnitOptions(RunSyncDB);
            var screen = new CLIScreen(options, selectionHistory, "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIController.ShowScreen(screen);
        }

        private Task RunSyncDB(int input, string selectionHistory)
        {
            using var context = ScaleUnitContext.CreateContext(GetScaleUnitId(input - 1));
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
