using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;

namespace CLI
{
    internal class SyncDB
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = RunSyncDB });
            }

            var screen = new CLIScreen(options, selectionHistory, "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static Task RunSyncDB(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                try
                {
                    new RunDBSync().Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occured while trying to run DbSync:\n{ex}");
                }
            }

            return Task.CompletedTask;
        }
    }
}
