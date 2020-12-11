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

        private async Task EnsureClientInitialized()
        {
            if (hubAosClient is null)
            {
                hubAosClient = await AOSClient.Construct(Config.HubAosResourceId(), Config.HubAosEndpoint());
            }
        }

        public async Task Install(int input, string selectionHistory)
        {
            await InstallWorkloadsOnHub();
            await WaitForWorkloadInstanceReadinessOnHub();

            Console.WriteLine("Done.");
        }

        public async Task InstallationStatus(int input, string selectionHistory)
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                List<WorkloadInstanceStatus> sysStatusList = new List<WorkloadInstanceStatus>();
                List<WorkloadInstanceStatus> nonSysStatusList = new List<WorkloadInstanceStatus>();
                List<ConfiguredWorkload> configuredWorkloads = Config.WorkloadList();
                List<string> sysIds = Config.SYSWorkloadInstanceIds();

                foreach (string workloadInstanceId in sysIds)
                {
                    sysStatusList.Add(await hubAosClient.CheckWorkloadStatus(workloadInstanceId));
                }

                foreach (ConfiguredWorkload configuredWorkload in configuredWorkloads)
                {
                    nonSysStatusList.Add(await hubAosClient.CheckWorkloadStatus(configuredWorkload.WorkloadInstanceId));
                }

                foreach (WorkloadInstanceStatus status in sysStatusList)
                {
                    Console.WriteLine($"SYS Workload installation status: {status.Health} {status.ErrorMessage}");
                }

                int count = 0;

                foreach (WorkloadInstanceStatus status in nonSysStatusList)
                {
                    Console.WriteLine($"{configuredWorkloads[count].Name} Id : {configuredWorkloads[count].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
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
                List<WorkloadInstanceStatus> sysStatusList = new List<WorkloadInstanceStatus>();
                List<WorkloadInstanceStatus> nonSysStatusList = new List<WorkloadInstanceStatus>();
                List<string> nonSysIds = Config.ConfiguredWorkloadInstanceIds();
                List<string> sysIds = Config.SYSWorkloadInstanceIds();

                foreach (string workloadInstanceId in sysIds)
                {
                    sysStatusList.Add(await hubAosClient.CheckWorkloadStatus(workloadInstanceId));
                }

                foreach (string workloadInstanceId in nonSysIds)
                {
                    nonSysStatusList.Add(await hubAosClient.CheckWorkloadStatus(workloadInstanceId));
                }

                foreach (WorkloadInstanceStatus sysStatus in sysStatusList)
                {
                    sysStatus.Should().NotBeNull("SYS workload instance should be found");
                    sysStatus.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
                }

                int count = 0;

                foreach (WorkloadInstanceStatus nonSysStatus in nonSysStatusList)
                {
                    nonSysStatus.Should().NotBeNull($"Non-SYS workload instance Id : {nonSysIds[count++]} should be found");
                    nonSysStatus.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
                }
            }, "Wait for workload instance readiness");
        }
    }
}
