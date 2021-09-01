using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadMover : AOSCommunicator
    {
        public WorkloadMover() : base() { }

        public async Task MoveWorkloads(string moveToId, DateTime movementDateTime)
        {
            await ReliableRun.Execute(async () =>
            {
                var aosClient = await GetScaleUnitAosClient();
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();

                foreach (var workloadInstance in workloadInstances)
                {
                    if (workloadInstance.ExecutingEnvironment.Any())
                    {
                        TemporalAssignment lastAssignment = workloadInstance.ExecutingEnvironment.Last();
                        if (lastAssignment?.Environment.ScaleUnitId == moveToId)
                        {
                            continue;
                        }
                    }
                    workloadInstance.ExecutingEnvironment.Add(CreateTemporalAssignment(moveToId, movementDateTime));
                }
                await aosClient.WriteWorkloadInstances(workloadInstances);
            }, $"Move workloads from scaleUnit {scaleUnit.ScaleUnitId} to hub");
        }

        public async Task ShowMovementStatus()
        {
            await ReliableRun.Execute(async () =>
            {
                var aosClient = await GetScaleUnitAosClient();
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();

                foreach (WorkloadInstance workloadInstance in workloadInstances)
                {
                    string name = workloadInstance.VersionedWorkload.Workload.Name;
                    string state = await GetMovementState(workloadInstance);
                    var movementState = new MovementState(state);

                    Console.WriteLine($"{name} Id : {workloadInstance.Id} Workload movement status: {movementState.GetStatus()}");
                }
            }, "Movement status");
        }

        private async Task<string> GetMovementState(WorkloadInstance workloadInstance)
        {
            var aosClient = await GetScaleUnitAosClient();
            TemporalAssignment lastAssignment = workloadInstance.ExecutingEnvironment.Last();
            return await aosClient.GetWorkloadMovementState(workloadInstance.Id, lastAssignment.EffectiveDate);
        }

        private TemporalAssignment CreateTemporalAssignment(string scaleUnitId, DateTime effectiveDate)
        {
            return new TemporalAssignment
            {
                EffectiveDate = effectiveDate,
                Environment = new PhysicalEnvironmentReference() { ScaleUnitId = scaleUnitId },
            };
        }
    }
}
