using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadDefinitionManager : AOSCommunicator
    {
        public WorkloadDefinitionManager() : base() { }

        public async Task UpgradeWorkloadsDefinition()
        {
            await ReliableRun.Execute(async () =>
            {
                IAOSClient scaleUnitAosClient = await GetScaleUnitAosClient();
                IAOSClient hubAosClient = await GetHubAosClient();

                var workloads = await hubAosClient.GetWorkloads();
                var workloadInstances = await scaleUnitAosClient.GetWorkloadInstances();

                await EnsureWorkloadsAreDrained(workloadInstances);

                foreach (var workloadInstance in workloadInstances)
                {
                    var workloadName = workloadInstance.VersionedWorkload.Workload.Name;
                    var workload = workloads.First(x => x.Name.Equals(workloadName));

                    workloadInstance.VersionedWorkload.Workload = workload;
                    workloadInstance.VersionedWorkload.Id = Guid.NewGuid().ToString();
                }

                var updatedInstances = await scaleUnitAosClient.WriteWorkloadInstances(workloadInstances);

            }, "Upgrading workloads");
        }

        private async Task EnsureWorkloadsAreDrained(List<WorkloadInstance> workloadInstances)
        {
            IAOSClient aosClient = await GetScaleUnitAosClient();

            foreach (var workloadInstance in workloadInstances)
            {
                if (!await WorkloadInstanceManager.IsWorkloadInStoppedState(aosClient, workloadInstance))
                {
                    throw new Exception($"Workload ${workloadInstance.VersionedWorkload.Workload.Name} on scale unit ${scaleUnit.ScaleUnitId} has not been drained");
                }
            }
        }
    }
}
