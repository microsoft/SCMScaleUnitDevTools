using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    class WorkloadMovementStatus
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = ShowWorkloadMovementStatusForScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Show status of workload movement on:\n", "\nEnvironment?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task ShowWorkloadMovementStatusForScaleUnit(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                await WorkloadMover.ShowMovementStatus();
            }
        }
    }
}
