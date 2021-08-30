using System;
using System.Linq;
using System.Threading.Tasks;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class WorkloadDefinitionManager : AOSCommunicator
    {
        public WorkloadDefinitionManager() : base() { }

        public async Task UpgradeWorkloadsDefinition()
        {
            await EnsureClientInitialized();

            var workloads = await aosClient.GetWorkloads();
            var workloadInstances = await aosClient.GetWorkloadInstances();

            foreach (var workloadInstance in workloadInstances)
            {
                var workloadName = workloadInstance.VersionedWorkload.Workload.Name;
                var workload = workloads.First(x => x.Name.Equals(workloadName));

                workloadInstance.VersionedWorkload.Workload = workload;
                workloadInstance.VersionedWorkload.Id = Guid.NewGuid().ToString();
            }

            var updatedInstances = await aosClient.WriteWorkloadInstances(workloadInstances);
        }
    }
}
