using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class ScaleUnitWorkloadInstaller : AOSCommunicator
    {
        public ScaleUnitWorkloadInstaller() : base() { }

        public async Task Install()
        {
            await InstallWorkloadsOnScaleUnit();

            Console.WriteLine("Done.");
        }

        public async Task InstallationStatus()
        {
            IAOSClient aosClient = await GetScaleUnitAosClient();
            var statusList = new List<WorkloadInstanceStatus>();
            List<WorkloadInstanceIdWithName> workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();
            int count = 0;

            foreach (WorkloadInstanceIdWithName workloadInstanceIdWithName in workloadInstanceIdWithNameList)
            {
                await ReliableRun.Execute(async () => statusList.Add(await WorkloadInstanceManager.GetWorkloadInstanceStatus(aosClient, workloadInstanceIdWithName.WorkloadInstanceId)), "Getting workload installation status");
            }
            foreach (WorkloadInstanceStatus status in statusList)
            {
                Console.WriteLine($"{workloadInstanceIdWithNameList[count].Name} Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
                count++;
            }
        }

        private async Task InstallWorkloadsOnScaleUnit()
        {
            IAOSClient scaleUnitAosClient = await GetScaleUnitAosClient();
            IAOSClient hubAosClient = await GetHubAosClient();

            List<WorkloadInstance> workloadInstances = null;

            await ReliableRun.Execute(async () => workloadInstances = await new WorkloadInstanceManager(hubAosClient).CreateWorkloadInstances(), "Create workload instances");

            int installedWorkloadCount = await CountInstalledWorkloads(scaleUnitAosClient);

            foreach (WorkloadInstance workloadInstance in workloadInstances)
            {
                if (!await WorkloadInstanceManager.IsWorkloadInstanceInInstallingState(scaleUnitAosClient, workloadInstance))
                {
                    Console.WriteLine($"Installing the {workloadInstance.VersionedWorkload.Workload.Name} workload");
                    var workloadInstanceToInstallList = new List<WorkloadInstance>() { workloadInstance };

                    await scaleUnitAosClient.WriteWorkloadInstances(workloadInstanceToInstallList);

                    if (WorkloadInstanceManager.IsSYSWorkload(workloadInstance) && installedWorkloadCount == 0)
                    {
                        ClearPotentiallyProblematicTables();
                    }
                }
                await WaitForWorkloadInstallation(workloadInstance);
            }
        }

        private async Task<int> CountInstalledWorkloads(IAOSClient scaleUnitAosClient)
        {
            List<WorkloadInstance> workloadInstances = null;

            await ReliableRun.Execute(async () => workloadInstances = await scaleUnitAosClient.GetWorkloadInstances(), "Get workload instances");

            int count = 0;

            foreach (WorkloadInstance workloadInstance in workloadInstances)
            {
                if (await WorkloadInstanceManager.IsWorkloadInstanceInReadyState(scaleUnitAosClient, workloadInstance))
                {
                    count++;
                }
            }
            return count;
        }

        private void ClearPotentiallyProblematicTables()
        {
            string sqlQuery = $@"
            USE {scaleUnit.AxDbName};
            EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '';

            TRUNCATE TABLE SysFeatureStateV0;
            TRUNCATE TABLE FeatureManagementState;
            TRUNCATE TABLE FeatureManagementMetadata;
            TRUNCATE TABLE SysFlighting;

            TRUNCATE TABLE NumberSequenceScope;
            TRUNCATE TABLE NumberSequenceReference;
            TRUNCATE TABLE NumberSequenceTable;

            EXEC sys.sp_set_session_context @key = N'ActiveScaleUnitId', @value = '{scaleUnit.ScaleUnitId}';
            ";

            var sqlQueryExecutor = new SqlQueryExecutor();
            sqlQueryExecutor.Execute(sqlQuery);
        }

        private async Task WaitForWorkloadInstallation(WorkloadInstance workloadInstance)
        {
            IAOSClient aosClient = await GetScaleUnitAosClient();

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
    }
}
