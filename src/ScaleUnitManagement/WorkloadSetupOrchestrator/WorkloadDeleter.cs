using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadDeleter
    {

        private AOSClient aosClient = null;
        private readonly ScaleUnitInstance scaleUnit;

        public WorkloadDeleter()
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


        public async Task DeleteWorkloadsFromScaleUnit()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();
                if (workloadInstances.Count == 0)
                {
                    Console.WriteLine($"No workloads to delete on scale unit {scaleUnit.ScaleUnitId}");
                }
                else
                {
                    List<WorkloadInstanceIdWithName> workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();

                    foreach (WorkloadInstance workloadInstance in workloadInstances)
                    {
                        WorkloadInstanceIdWithName workload = workloadInstanceIdWithNameList.Find(wl => wl.WorkloadInstanceId == workloadInstance.Id);
                        if (workload != null)
                        {
                            Console.WriteLine($"Deleting {workload.Name} Id: {workload.WorkloadInstanceId}");
                        }
                    }
                    _ = await aosClient.DeleteWorkloadInstances(workloadInstances);
                }
            }, $"Delete workloads from scale unit {scaleUnit.ScaleUnitId}");
        }
    }
}
