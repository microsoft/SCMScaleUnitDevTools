using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;

namespace CLI
{
    class MoveWorkloads
    {
        public static async Task MoveAllWorkloads(int input, string selectionHistory)
        {
            try
            {
                Console.WriteLine("Moving all workloads to the hub");
                List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();

                foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
                {
                    if (!scaleUnit.IsHub())
                    {
                        await MoveWorkloadsFromScaleUnitToHub(scaleUnit);
                    }
                }
                Console.WriteLine("done");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to move workloads:\n{ex}");
            }
        }

        private static async Task MoveWorkloadsFromScaleUnitToHub(ScaleUnitInstance scaleUnit)
        {
            string hubId = "@@";
            ScaleUnitInstance hub = Config.HubScaleUnit();
            DateTime effectiveTime = DateTime.UtcNow.AddMinutes(5);
            await WorkloadMover.MoveWorkloads(hubId, hub, effectiveTime);
            await WorkloadMover.MoveWorkloads(hubId, scaleUnit, effectiveTime);
        }
    }
}
