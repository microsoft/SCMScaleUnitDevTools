using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System;

namespace CLI
{
    class CleanUpBlobs
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = CleanUpScaleUnitBlob });
            }

            var screen = new CLIScreen(options, selectionHistory, "Please select the scale unit you would like to clean up:\n", "\nScale Unit to clean up: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task CleanUpScaleUnitBlob(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                var blobCleaner = new BlobCleaner();
                await blobCleaner.CleanBlob();
            }

            Console.WriteLine("Done");
        }
    }
}
