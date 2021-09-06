using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class PipelineManager : AOSCommunicator
    {
        public PipelineManager() : base() { }

        public async Task DrainWorkloadDataPipelines()
        {
            var aosClient = await GetScaleUnitAosClient();
            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await aosClient.GetWorkloadInstances(), "Getting workload instances");

            foreach (var workloadInstance in workloadInstances)
            {
                /* Bug #614217 in the AX causes the client to fail if the SYS workload is stopped and started repeatedly on the spoke. 
                    * Since the SYS workload on the spoke never sends any packages, draining and starting it can be skipped.
                    * This should be solved in AX version 10.0.23.
                    */
                if (WorkloadInstanceManager.IsWorkloadSYSOnSpoke(workloadInstance))
                {
                    Console.WriteLine($"Skipping the SYS workload on {scaleUnit.PrintableName()}");
                    continue;
                }
                await ReliableRun.Execute(async () => await aosClient.DrainWorkload(workloadInstance.Id), "Draining workload instance");
            }

            foreach (var workloadInstance in workloadInstances)
            {
                if (WorkloadInstanceManager.IsWorkloadSYSOnSpoke(workloadInstance))
                {
                    continue;
                }
                Console.WriteLine($"Waiting for {workloadInstance.VersionedWorkload.Workload.Name} to be fully drained on {scaleUnit.PrintableName()}");
                await WaitForWorkloadDraining(workloadInstance);
            }
        }

        public async Task StartWorkloadDataPipelines()
        {
            var aosClient = await GetScaleUnitAosClient();
            List<WorkloadInstance> workloadInstances = null;
            await ReliableRun.Execute(async () => workloadInstances = await aosClient.GetWorkloadInstances(), "Getting workload instances");

            foreach (var workloadInstance in workloadInstances)
            {
                if (WorkloadInstanceManager.IsWorkloadSYSOnSpoke(workloadInstance))
                {
                    Console.WriteLine($"Skipping the SYS workload on {scaleUnit.PrintableName()}");
                    continue;
                }
                Console.WriteLine($"Starting the {workloadInstance.VersionedWorkload.Workload.Name} workload on {scaleUnit.PrintableName()}");
                await ReliableRun.Execute(async () => await aosClient.StartWorkload(workloadInstance.Id), "Starting workload instance");
            }
        }

        public async Task WaitForWorkloadDraining(WorkloadInstance workloadInstance)
        {
            IAOSClient aosClient = await GetScaleUnitAosClient();
            if (await WorkloadInstanceManager.IsWorkloadInStoppedState(aosClient, workloadInstance))
                return;

            int count = 0;
            int queryInterval = 10;
            do
            {
                for (int i = 0; i < queryInterval; i++)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.Write(".");
                }

                count++;
                if (count == 300 / queryInterval) // After five minutes
                {
                    Console.WriteLine($"\nThis is taking a long time.");
                    Console.WriteLine($"Still waiting for the {workloadInstance.VersionedWorkload.Workload.Name} workload to be drained on {scaleUnit.PrintableName()}");
                }

            } while (!await WorkloadInstanceManager.IsWorkloadInStoppedState(aosClient, workloadInstance));

            Console.WriteLine();
        }
    }
}
