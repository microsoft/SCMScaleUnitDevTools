using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;
using CLIFramework;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    internal class DeleteWorkloads
    {
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = DeleteWorkloadsFromScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to delete all workloads from?: ");
            await CLIMenu.ShowScreen(screen);
        }

        public static async Task DeleteWorkloadsFromScaleUnit(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                WorkloadDeleter workloadDeleter = new WorkloadDeleter();
                await workloadDeleter.DeleteWorkloadsFromScaleUnit();
            }
            Console.WriteLine("Done.");
        }
    }
}
