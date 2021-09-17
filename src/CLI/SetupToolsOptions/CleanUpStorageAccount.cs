using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System;

namespace CLI.SetupToolsOptions
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
            try
            {
                using var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId);
                var storageAccountManager = new StorageAccountManager();
                await storageAccountManager.CleanStorageAccount();
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An exception occured while cleaning up storage: {ex.Message}");
            }
        }
    }
}
