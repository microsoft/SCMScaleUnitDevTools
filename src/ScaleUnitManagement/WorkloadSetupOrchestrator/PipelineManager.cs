using System;
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
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
           {
               var workloadInstances = await aosClient.GetWorkloadInstances();
               foreach (var workloadInstance in workloadInstances)
               {
                   if (SYSWorkloadOnSpoke(workloadInstance))
                   {
                       continue;
                   }
                   await aosClient.DrainWorkload(workloadInstance.Id);
               }

               foreach (var workloadInstance in workloadInstances)
               {
                   if (SYSWorkloadOnSpoke(workloadInstance))
                   {
                       Console.WriteLine($"Skipping the SYS workload on ${scaleUnit.PrintableName()}");
                       continue;
                   }
                   Console.WriteLine($"Draining the {workloadInstance.VersionedWorkload.Workload.Name} workload on ${scaleUnit.PrintableName()}");
                   await WaitForWorkloadDraining(workloadInstance);
               }
           }, "Draining workload data pipelines");
        }

        public async Task StartWorkloadDataPipelines()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
           {
               var workloadInstances = await aosClient.GetWorkloadInstances();
               foreach (var workloadInstance in workloadInstances)
               {
                   if (SYSWorkloadOnSpoke(workloadInstance))
                   {
                       Console.WriteLine($"Skipping the SYS workload on ${scaleUnit.PrintableName()}");
                       continue;
                   }
                   Console.WriteLine($"Starting the {workloadInstance.VersionedWorkload.Workload.Name} workload on ${scaleUnit.PrintableName()}");
                   await aosClient.StartWorkload(workloadInstance.Id);
               }
           }, "Starting workload data pipelines");
        }

        public async Task WaitForWorkloadDraining(WorkloadInstance workloadInstance)
        {
            if (!await WorkloadInstanceManager.IsWorkloadBeingDrained(aosClient, workloadInstance))
                return;

            int count = 0;
            do
            {
                for (int i = 0; i < 10; i++) // wait 10 seconds before querying the status again.
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.Write(".");
                }

                count++;
                if (count == 30) // After five minutes
                {
                    Console.WriteLine($"\nThis is taking a long time.");
                    Console.WriteLine($"Still waiting for the {workloadInstance.VersionedWorkload.Workload.Name} workload to be drained on ${scaleUnit.PrintableName()}");
                }

            } while (await WorkloadInstanceManager.IsWorkloadBeingDrained(aosClient, workloadInstance));

            Console.WriteLine();
        }

        private bool SYSWorkloadOnSpoke(WorkloadInstance workloadInstance)
        {
            var name = workloadInstance.VersionedWorkload.Workload.Name;
            return name.Equals("SYS") && !scaleUnit.IsHub();
        }
    }
}
