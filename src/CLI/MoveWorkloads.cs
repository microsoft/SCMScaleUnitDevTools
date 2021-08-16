using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;

namespace CLI
{
    internal class MoveWorkloads
    {
        public static async Task MoveAllWorkloads(int input, string selectionHistory)
        {
            try
            {
                Console.WriteLine("Moving all workloads to the hub");
                List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();

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

        private static async Task MoveWorkloadsFromScaleUnitToHub(ScaleUnitInstance scaleUnit)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            using (var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId))
            {
                string hubId = "@@";
                ScaleUnitInstance hub = Config.HubScaleUnit();
                DateTime effectiveTime = DateTime.UtcNow.AddMinutes(5);
                WorkloadMover workloadMover = new WorkloadMover();
                await workloadMover.MoveWorkloads(hubId, effectiveTime);
            }
        }
    }
}
