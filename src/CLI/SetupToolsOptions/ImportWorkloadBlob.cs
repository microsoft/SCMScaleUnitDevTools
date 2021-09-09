using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System;

namespace CLI.SetupToolsOptions
{
    internal class ImportWorkloadBlob
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (var scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = ImportWorkloadBlobFromSasToken });
            }

            var screen = new CLIScreen(options, selectionHistory, "Please select the scale unit you would like to import workloads to:\n", "\nScale unit storage to import to: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task ImportWorkloadBlobFromSasToken(int input, string selectionHistory)
        {
            var sasToken = CLIMenu.EnterValuePrompt("Please paste in the blob SAS URL for the blob that the workloads should be copied from:");
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
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
}
