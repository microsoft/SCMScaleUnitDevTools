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
        public static async Task DeleteWorkloadsFromHub()
        {
            await ReliableRun.Execute(async () =>
            {
                AOSClient aosClient = await AOSClient.Construct(Config.HubScaleUnit());
                List<WorkloadInstance> workloadInstances = await aosClient.GetWorkloadInstances();
                if (workloadInstances.Count == 0)
                {
                    Console.WriteLine("No workloads to delete on the hub");
                }
                else
                {
                    _ = await aosClient.DeleteWorkloadInstances(workloadInstances);
                }
            }, "Move workloads back to hub");
        }

    }
}
