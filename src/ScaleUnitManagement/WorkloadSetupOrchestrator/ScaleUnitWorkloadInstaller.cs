using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class ScaleUnitWorkloadInstaller : AOSEndpoint
    {
        public ScaleUnitWorkloadInstaller() : base() { }

        public async Task Install()
        {
            await InstallWorkloadsOnScaleUnit();

            Console.WriteLine("Done.");
        }

        public async Task InstallationStatus()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstanceStatus> statusList = new List<WorkloadInstanceStatus>();
                List<WorkloadInstanceIdWithName> workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();
                int count = 0;

                foreach (WorkloadInstanceIdWithName workloadInstanceIdWithName in workloadInstanceIdWithNameList)
                {
                    statusList.Add(await WorkloadInstanceManager.GetWorkloadInstanceStatus(aosClient, workloadInstanceIdWithName.WorkloadInstanceId));
                }
                foreach (WorkloadInstanceStatus status in statusList)
                {
                    Console.WriteLine($"{workloadInstanceIdWithNameList[count].Name} Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
                    count++;
                }
            }, "Installation status");
        }

        private async Task WaitForWorkloadInstallation(WorkloadInstance workloadInstance)
        {
            if (!await WorkloadInstanceManager.IsWorkloadInstanceInInstallingState(aosClient, workloadInstance))
                return;

            Console.WriteLine($"Waiting for the {workloadInstance.VersionedWorkload.Workload.Name} workload initial sync to complete");

            int count = 0;
            do
            {
                for (int i = 0; i < 10; i++) // wait 10 seconds before querying the status again.
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));
                    Console.Write(".");
                }

                count++;
                if (count == 12) // After two minutes
                {
                    Console.WriteLine($"\nThis is taking a long time. Ensure the upload packages, for the {workloadInstance.VersionedWorkload.Workload.Name} workload, haven't failed.");
                    Console.WriteLine($"Waiting for the {workloadInstance.VersionedWorkload.Workload.Name} workload initial sync to complete");
                }

            } while (!await WorkloadInstanceManager.IsWorkloadInstanceInReadyState(aosClient, workloadInstance));

            Console.WriteLine();
        }

        private async Task InstallWorkloadsOnScaleUnit()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                IAOSClient scaleUnitAosClient = aosClient;
                AOSClient hubAosClient = await AOSClient.Construct(Config.HubScaleUnit());

                List<WorkloadInstance> workloadInstances = await new WorkloadInstanceManager(hubAosClient).CreateWorkloadInstances();

                foreach (var workloadInstance in workloadInstances)
                {
                    if (await WorkloadInstanceManager.IsWorkloadInstanceInReadyState(scaleUnitAosClient, workloadInstance))
                        continue;

                    if (!await WorkloadInstanceManager.IsWorkloadInstanceInInstallingState(scaleUnitAosClient, workloadInstance))
                    {
                        Console.WriteLine($"Installing the {workloadInstance.VersionedWorkload.Workload.Name} workload");
                        List<WorkloadInstance> workloadInstanceToInstallList = new List<WorkloadInstance>() { workloadInstance };
                        await scaleUnitAosClient.WriteWorkloadInstances(workloadInstanceToInstallList);
                    }

                    // Assuming that the LBD environment will be on the app version >= 10.0.17
                    if (scaleUnit.EnvironmentType == EnvironmentType.LBD || AxDeployment.IsApplicationVersionMoreRecentThan("10.8.581.0"))
                        await WaitForWorkloadInstallation(workloadInstance);

                }
            }, "Install workload on scale unit");
        }
    }
}
