using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;

namespace CLI.WorkloadMovementOptions
{
    internal class MoveWorkloads
    {
        public async Task MoveAllWorkloads(int input, string selectionHistory)
        {
            try
            {
                Console.WriteLine("Moving all workloads to the hub");
                System.Collections.Generic.List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();

                foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
                {
                    await MoveWorkloadsFromScaleUnitToHub(scaleUnit);
                }
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to move workloads:\n{ex}");
            }
        }

        private async Task MoveWorkloadsFromScaleUnitToHub(ScaleUnitInstance scaleUnit)
        {
            using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
            string hubId = "@@";
            var workloadMover = new WorkloadMover();
            await workloadMover.MoveWorkloads(hubId);
        }
    }
}
