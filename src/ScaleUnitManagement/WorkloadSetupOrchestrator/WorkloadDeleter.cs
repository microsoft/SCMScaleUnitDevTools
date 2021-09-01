using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadDeleter : AOSCommunicator
    {
        public WorkloadDeleter() : base() { }

        public async Task DeleteWorkloadsFromScaleUnit()
        {
            await ReliableRun.Execute(async () =>
            {
                IAOSClient aosClient = await GetScaleUnitAosClient();
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();
                if (workloadInstances.Count == 0)
                {
                    Console.WriteLine($"No workloads to delete on scale unit {scaleUnit.ScaleUnitId}");
                    return;
                }

                foreach (WorkloadInstance workloadInstance in workloadInstances)
                {
                    string name = workloadInstance.VersionedWorkload.Workload.Name;
                    Console.WriteLine($"Deleting {name} Id: {workloadInstance.Id}");
                }
                _ = await aosClient.DeleteWorkloadInstances(workloadInstances);
            }, $"Delete workloads from scale unit {scaleUnit.ScaleUnitId}");
        }
    }
}
