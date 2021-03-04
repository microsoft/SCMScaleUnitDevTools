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
    public class HubWorkloadInstaller
    {
        private AOSClient hubAosClient = null;
        private readonly ScaleUnitInstance scaleUnit;

        public HubWorkloadInstaller()
        {
            scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
        }

        private async Task EnsureClientInitialized()
        {
            if (hubAosClient is null)
            {
                hubAosClient = await AOSClient.Construct(scaleUnit);
            }
        }

        public async Task Install()
        {
            await InstallWorkloadsOnHub();
            await WaitForWorkloadInstanceReadinessOnHub();

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
                    statusList.Add(await hubAosClient.CheckWorkloadStatus(workloadInstanceIdWithName.WorkloadInstanceId));
                }
                foreach (WorkloadInstanceStatus status in statusList)
                {
                    Console.WriteLine($"{workloadInstanceIdWithNameList[count].Name} Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
                    count++;
                }
            }, "Installation status");
        }

        private async Task InstallWorkloadsOnHub()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstance> workloadInstances = await new WorkloadInstanceManager(hubAosClient).CreateWorkloadInstances();
                List<WorkloadInstance> createdInstances = await hubAosClient.WriteWorkloadInstances(workloadInstances);

                this.ValidateCreatedWorkloadInstances(workloadInstances, createdInstances);
            }, "Install workloads on hub");
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
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstanceStatus> statusList = new List<WorkloadInstanceStatus>();
                List<WorkloadInstanceIdWithName> workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();
                int count = 0;

                foreach (WorkloadInstanceIdWithName workloadInstanceIdWithName in workloadInstanceIdWithNameList)
                {
                    statusList.Add(await hubAosClient.CheckWorkloadStatus(workloadInstanceIdWithName.WorkloadInstanceId));
                }

                foreach (WorkloadInstanceStatus status in statusList)
                {
                    status.Should().NotBeNull($"{workloadInstanceIdWithNameList[count].Name} workload instance Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} should be found");
                    status.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
                    count++;
                }

            }, "Wait for workload instance readiness");
        }
    }
}
