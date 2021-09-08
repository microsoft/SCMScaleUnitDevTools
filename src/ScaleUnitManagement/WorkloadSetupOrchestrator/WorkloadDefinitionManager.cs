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
            var scaleUnitAosClient = await GetScaleUnitAosClient();
            var hubAosClient = await GetHubAosClient();

            List<Workload> workloads = null;
            await ReliableRun.Execute(async () => workloads = await hubAosClient.GetWorkloads(), "Getting workloads");

            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await scaleUnitAosClient.GetWorkloadInstances(), "Getting workload instances");

            await EnsureWorkloadsAreDrained(workloadInstances);

            foreach (var workloadInstance in workloadInstances)
            {
                var workloadName = workloadInstance.VersionedWorkload.Workload.Name;
                var workload = workloads.First(x => x.Name.Equals(workloadName));

                workloadInstance.VersionedWorkload.Workload = workload;
                workloadInstance.VersionedWorkload.Id = Guid.NewGuid().ToString();

                await ReliableRun.Execute(async () => await scaleUnitAosClient.WriteWorkloadInstances(new List<WorkloadInstance> { workloadInstance }), "Writing workload instance");
            }
        }

        private async Task EnsureWorkloadsAreDrained(List<WorkloadInstance> workloadInstances)
        {
            var aosClient = await GetScaleUnitAosClient();

            foreach (var workloadInstance in workloadInstances)
            {
                if (WorkloadInstanceManager.IsWorkloadSYSOnSpoke(workloadInstance))
                {
                    continue;
                }

                if (!await WorkloadInstanceManager.IsWorkloadInStoppedState(aosClient, workloadInstance))
                {
                    throw new Exception($"Workload {workloadInstance.VersionedWorkload.Workload.Name} on scale unit {scaleUnit.ScaleUnitId} has not been drained");
                }
            }
        }
    }
}
