using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadDeleter
    {
        public static async Task DeleteWorkloadsFromScaleUnit()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            await ReliableRun.Execute(async () =>
            {
                AOSClient aosClient = await AOSClient.Construct(scaleUnit);
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
