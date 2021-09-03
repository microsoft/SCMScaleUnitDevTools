using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;
using DeepEqual.Syntax;
using FluentAssertions;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class HubWorkloadInstaller : AOSCommunicator
    {
        public HubWorkloadInstaller() : base() { }

        public async Task Install()
        {
            await InstallWorkloadsOnHub();
            await WaitForWorkloadInstanceReadinessOnHub();

            Console.WriteLine("Done.");
        }

        public async Task InstallationStatus()
        {
            var aosClient = await GetScaleUnitAosClient();
            var statusList = new List<WorkloadInstanceStatus>();
            var workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();
            int count = 0;
            foreach (var workloadInstanceIdWithName in workloadInstanceIdWithNameList)
            {
                await ReliableRun.Execute(async () => statusList.Add(await aosClient.CheckWorkloadStatus(workloadInstanceIdWithName.WorkloadInstanceId)), "Checking workload status");
            }
            foreach (var status in statusList)
            {
                Console.WriteLine($"{workloadInstanceIdWithNameList[count].Name} Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
                count++;
            }
        }

        private async Task InstallWorkloadsOnHub()
        {
            var aosClient = await GetScaleUnitAosClient();
            var workloadInstances = await new WorkloadInstanceManager(aosClient).CreateWorkloadInstances();

            List<WorkloadInstance> createdInstances = null;
            foreach (var workloadInstance in workloadInstances)
            {
                await ReliableRun.Execute(async () => createdInstances = await aosClient.WriteWorkloadInstances(new List<WorkloadInstance> { workloadInstance }), "Installing workload instance");
            }

            this.ValidateCreatedWorkloadInstances(workloadInstances, createdInstances);
        }

        private void ValidateCreatedWorkloadInstances(List<WorkloadInstance> expectedWorkloadInstances, List<WorkloadInstance> createdInstances)
        {
            createdInstances.Should().NotBeNull();

            foreach (string workloadInstanceId in Config.AllWorkloadInstanceIds())
            {
                var workloadInstance = expectedWorkloadInstances.Find((instance) => instance.Id.Equals(workloadInstanceId));
                var foundWorkloadInstance = createdInstances.Find((instance) => instance.Id.Equals(workloadInstanceId));

                foundWorkloadInstance.Should().NotBeNull();

                foundWorkloadInstance.WithDeepEqual(workloadInstance).Assert();
            }
        }

        private async Task WaitForWorkloadInstanceReadinessOnHub()
        {
            var aosClient = await GetScaleUnitAosClient();
            var statusList = new List<WorkloadInstanceStatus>();
            var workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();
            int count = 0;

            foreach (var workloadInstanceIdWithName in workloadInstanceIdWithNameList)
            {
                await ReliableRun.Execute(async () => statusList.Add(await aosClient.CheckWorkloadStatus(workloadInstanceIdWithName.WorkloadInstanceId)), "Checking workload status");
            }

            foreach (var status in statusList)
            {
                status.Should().NotBeNull($"{workloadInstanceIdWithNameList[count].Name} workload instance Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} should be found");
                status.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
                count++;
            }
        }
    }
}
