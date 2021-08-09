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
        private AOSClient aosClient = null;
        private readonly ScaleUnitInstance scaleUnit;

        public WorkloadMover()
        {
            this.scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
        }

        private async Task EnsureClientInitialized()
        {
            if (aosClient is null)
            {
                aosClient = await AOSClient.Construct(scaleUnit);
            }
        }

        public async Task MoveWorkloads(string moveToId, DateTime movementDateTime)
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();
                DateTime now = DateTime.UtcNow;

                foreach (WorkloadInstance workloadInstance in workloadInstances)
                {
                    if (workloadInstance.ExecutingEnvironment.Count() > 0)
                    {
                        TemporalAssignment lastAssignment = workloadInstance.ExecutingEnvironment.Last();
                        if (lastAssignment.Environment.ScaleUnitId == scaleUnit.ScaleUnitId)
                        {
                            continue;
                        }
                    }
                    workloadInstance.ExecutingEnvironment.Add(CreateTemporalAssignment(moveToId, movementDateTime));
                }
                await aosClient.WriteWorkloadInstances(workloadInstances);
            }, $"Move workloads from scaleUnit {scaleUnit.ScaleUnitId} back to hub");
        }

        public async Task ShowMovementStatus()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
                AOSClient aosClient = await AOSClient.Construct(scaleUnit);
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();
                List<MovementState> stateList = new List<MovementState>();
                List<WorkloadInstanceIdWithName> workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();

                foreach (WorkloadInstance workloadInstance in workloadInstances)
                {
                    string state = await getMovementState(aosClient, workloadInstance);
                    stateList.Add(new MovementState(state));
                }

                int count = 0;
                foreach (MovementState state in stateList)
                {
                    Console.WriteLine($"{workloadInstanceIdWithNameList[count].Name} Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} Workload installation status: {state.getStatus()}");
                    count++;
                }
            }, "Movement status");
        }

        private async Task<string> getMovementState(AOSClient aosClient, WorkloadInstance workloadInstance)
        {
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
