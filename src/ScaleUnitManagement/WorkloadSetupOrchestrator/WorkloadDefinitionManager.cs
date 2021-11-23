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
            IAOSClient scaleUnitAosClient = await GetScaleUnitAosClient();

            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await scaleUnitAosClient.GetWorkloadInstances(), "Getting workload instances");

            await EnsureWorkloadsAreDrained(workloadInstances, scaleUnitAosClient);

            IAOSClient hubAosClient = await GetHubAosClient();

            List<Workload> workloads = null;
            await ReliableRun.Execute(async () => workloads = await hubAosClient.GetWorkloads(), "Getting workloads");
            workloads.PruneRedundancies();

            foreach (WorkloadInstance workloadInstance in workloadInstances)
            {
                string workloadName = workloadInstance.VersionedWorkload.Workload.Name;
                Workload workload = workloads.First(x => x.Name.Equals(workloadName));

                workloadInstance.VersionedWorkload.Workload = workload;
                workloadInstance.VersionedWorkload.Id = Guid.NewGuid().ToString();

                Console.WriteLine($"Upgrading the definition of {workloadName} workload on {scaleUnit.PrintableName()}");

                await ReliableRun.Execute(async () => await scaleUnitAosClient.WriteWorkloadInstances(new List<WorkloadInstance> { workloadInstance }), "Writing workload instance");
            }
        }

        private async Task EnsureWorkloadsAreDrained(List<WorkloadInstance> workloadInstances, IAOSClient aosClient)
        {
            foreach (WorkloadInstance workloadInstance in workloadInstances)
            {
                if (WorkloadInstanceManager.IsSYSWorkloadOnSpoke(workloadInstance))
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
