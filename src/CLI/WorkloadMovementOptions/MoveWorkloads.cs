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
                var scaleUnitInstances = Config.ScaleUnitInstances();

                foreach (var scaleUnit in scaleUnitInstances)
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
            var hubId = "@@";
            var hub = Config.HubScaleUnit();
            var effectiveTime = DateTime.UtcNow.AddMinutes(5);
            var workloadMover = new WorkloadMover();
            await workloadMover.MoveWorkloads(hubId, effectiveTime);
        }
    }
}
