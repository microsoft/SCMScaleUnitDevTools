using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadMover
    {
        public static async Task MoveWorkloads(string moveToId, ScaleUnitInstance targetScaleUnit, DateTime movementDateTime)
        {
            await ReliableRun.Execute(async () =>
            {
                AOSClient aosClient = await AOSClient.Construct(targetScaleUnit);
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();
                DateTime now = DateTime.UtcNow;

                foreach (WorkloadInstance workloadInstance in workloadInstances)
                {
                    if (workloadInstance.ExecutingEnvironment.Count() > 0)
                    {
                        TemporalAssignment lastAssignment = workloadInstance.ExecutingEnvironment.Last();
                        if (lastAssignment.Environment.ScaleUnitId == targetScaleUnit.ScaleUnitId)
                        {
                            continue;
                        }
                    }
                    workloadInstance.ExecutingEnvironment.Add(CreateTemporalAssignment(moveToId, movementDateTime));
                }
                
                await aosClient.WriteWorkloadInstances(workloadInstances);
            }, "Move workloads back to hub");
        }


        private static TemporalAssignment CreateTemporalAssignment(string scaleUnitId, DateTime effectiveDate)
        {
            return new TemporalAssignment
            {
                EffectiveDate = effectiveDate,
                Environment = new PhysicalEnvironmentReference() { ScaleUnitId = scaleUnitId },
            };
        }
    }
}
