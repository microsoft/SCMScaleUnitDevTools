using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System;

namespace CLI
{
    internal class CleanUpStorageAccount
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = CleanUpScaleUnitStorageAccount });
            }

            var screen = new CLIScreen(options, selectionHistory, "Please select the scale unit you would like to clean the storage account of:\n", "\nScale unit storage to clean up: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task CleanUpScaleUnitStorageAccount(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                var accountCleaner = new StorageAccountCleaner();
                await accountCleaner.CleanStorageAccount();
            }

            Console.WriteLine("Done");
        }
    }
}
