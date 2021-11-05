using System.Collections.Generic;
using CloudAndEdgeLibs;
using CloudAndEdgeLibs.Contracts;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    public static class WorkloadExtensions
    {
        public static void PruneRedundancies(this List<Workload> workloads)
        {
            foreach (Workload workload in workloads)
            {
                workload.__DependentSystemMetadata = new SortedSet<SystemMetadata>();
            }
        }
    }
}
