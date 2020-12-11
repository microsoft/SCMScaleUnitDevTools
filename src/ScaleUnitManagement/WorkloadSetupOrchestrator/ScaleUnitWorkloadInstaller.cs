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
    public class ScaleUnitWorkloadInstaller
    {
        private AOSClient scaleUnitAosClient = null;

        private async Task EnsureClientInitialized()
        {
            if (scaleUnitAosClient is null)
            {
                scaleUnitAosClient = await AOSClient.Construct(Config.ScaleUnitAosResourceId(), Config.ScaleUnitAosEndpoint());
            }
        }

        public async Task Install(int input, string selectionHistory)
        {
            await InstallWorkloadsOnScaleUnit();

            Console.WriteLine("Done.");
        }

        public async Task InstallationStatus(int input, string selectionHistory)
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstanceStatus> statusList = new List<WorkloadInstanceStatus>();
                List<WorkloadInstanceIdWithName> workloadInstanceIdWithNameList = Config.WorkloadInstanceIdWithNameList();
                int count = 0;

                foreach (WorkloadInstanceIdWithName workloadInstanceIdWithName in workloadInstanceIdWithNameList)
                {
                    statusList.Add(await scaleUnitAosClient.CheckWorkloadStatus(workloadInstanceIdWithName.WorkloadInstanceId));
                }
                foreach (WorkloadInstanceStatus status in statusList)
                {
                    Console.WriteLine($"{workloadInstanceIdWithNameList[count].Name} Id : {workloadInstanceIdWithNameList[count].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
                    count++;
                }
            }, "Installation status");
        }

        private async Task InstallWorkloadsOnScaleUnit()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                AOSClient hubAosClient = await AOSClient.Construct(Config.HubAosResourceId(), Config.HubAosEndpoint());

                List<WorkloadInstance> workloadInstances = await new WorkloadInstanceManager(hubAosClient).CreateWorkloadInstances();
                List<WorkloadInstance> createdInstances = await scaleUnitAosClient.WriteWorkloadInstances(workloadInstances);

                this.ValidateCreatedWorkloadInstances(workloadInstances, createdInstances);
            }, "Install workload on scale unit");
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
    }
}
