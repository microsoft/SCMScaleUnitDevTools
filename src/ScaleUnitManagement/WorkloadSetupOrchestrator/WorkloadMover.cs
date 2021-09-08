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
            var aosClient = await GetScaleUnitAosClient();
            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await aosClient.GetWorkloadInstances(), "Getting workload instances");

            foreach (var workloadInstance in workloadInstances)
            {
                if (workloadInstance.ExecutingEnvironment.Any())
                {
                    var lastAssignment = workloadInstance.ExecutingEnvironment.Last();
                    if (moveToId.Equals(lastAssignment?.Environment.ScaleUnitId))
                    {
                        continue;
                    }
                }
                workloadInstance.ExecutingEnvironment.Add(CreateTemporalAssignment(moveToId, movementDateTime));

                await ReliableRun.Execute(async () => await aosClient.WriteWorkloadInstances(new List<WorkloadInstance> { workloadInstance }), $"Moving workload instance from scale unit {scaleUnit.ScaleUnitId} to the hub");
            }
        }

        public async Task ShowMovementStatus()
        {
            var aosClient = await GetScaleUnitAosClient();
            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await aosClient.GetWorkloadInstances(), "Getting workload instances");

            foreach (var workloadInstance in workloadInstances)
            {
                var name = workloadInstance.VersionedWorkload.Workload.Name;
                var state = await GetMovementState(workloadInstance);
                var movementState = new MovementState(state);

                Console.WriteLine($"{name} Id : {workloadInstance.Id} Workload movement status: {movementState.GetStatus()}");
            }
        }

        private async Task<string> GetMovementState(WorkloadInstance workloadInstance)
        {
            var aosClient = await GetScaleUnitAosClient();
            string state = null;
            var lastAssignment = workloadInstance.ExecutingEnvironment.Last();
            await ReliableRun.Execute(async () => state = await aosClient.GetWorkloadMovementState(workloadInstance.Id, lastAssignment.EffectiveDate), "Getting movement state");
            return state;
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
