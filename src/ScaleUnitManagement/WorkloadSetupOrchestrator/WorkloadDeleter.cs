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
            var aosClient = await GetScaleUnitAosClient();
            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await aosClient.GetWorkloadInstances(), "Getting workload instances");
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
            await ReliableRun.Execute(async () => await aosClient.DeleteWorkloadInstances(workloadInstances), "Deleting workload instances");
        }
    }
}
