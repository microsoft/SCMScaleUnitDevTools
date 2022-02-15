using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class MoveAllWorkloadsAction : IAction
    {
        public async Task Execute()
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

        private async Task MoveWorkloadsFromScaleUnitToHub(ScaleUnitInstance scaleUnit)
        {
            using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
            var workloadMover = new WorkloadMover();
            await workloadMover.MoveWorkloadsToHub();
        }
    }
}
