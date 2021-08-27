using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;

namespace CLI
{
    internal class DisableScaleUnitFeature
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = RunDisableScaleUnitFeature });
            }

            var screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to disable scale unit feature on?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static Task RunDisableScaleUnitFeature(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                try
                {
                    new RunDBSync().Run(false);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"An error occured while trying to disable scale unit feature:\n{ex}");
                }
            }

            return Task.CompletedTask;
        }
    }
}
