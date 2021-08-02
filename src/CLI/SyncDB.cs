using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;
using System;

namespace CLI
{
    class SyncDB
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = RunSyncDB });
            }

            var screen = new CLIScreen(options, "Home", "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task RunSyncDB(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();
            ScaleUnitDBSync scaleUnitDBSync = new ScaleUnitDBSync();
            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                try
                {
                    await scaleUnitDBSync.Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occured while trying to run DbSync:\n{ex}");
                }
            }
        }
    }
}
