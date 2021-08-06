using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;
using CLIFramework;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    class DeleteWorkloads
    {

        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = DeleteWorkloadsFromScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to delete all workloads from?: ");
            await CLIMenu.ShowScreen(screen);
        }

        public static async Task DeleteWorkloadsFromScaleUnit(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                await WorkloadDeleter.DeleteWorkloadsFromScaleUnit();
            }
            Console.WriteLine("Done.");
        }
    }
}
