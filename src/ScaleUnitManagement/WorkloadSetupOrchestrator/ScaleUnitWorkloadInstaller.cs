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
                List<WorkloadInstanceStatus> sysStatusList = new List<WorkloadInstanceStatus>();
                List<WorkloadInstanceStatus> nonSysStatusList = new List<WorkloadInstanceStatus>();
                List<ConfiguredWorkload> configuredWorkloads = Config.WorkloadList();
                List<string> sysIds = Config.SYSWorkloadInstanceIds();

                foreach (string workloadInstanceId in sysIds)
                {
                    sysStatusList.Add(await scaleUnitAosClient.CheckWorkloadStatus(workloadInstanceId));
                }

                foreach (ConfiguredWorkload configuredWorkload in configuredWorkloads)
                {
                    nonSysStatusList.Add(await scaleUnitAosClient.CheckWorkloadStatus(configuredWorkload.WorkloadInstanceId));
                }

                foreach (WorkloadInstanceStatus status in sysStatusList)
                {
                    Console.WriteLine($"SYS Workload installation status: {status.Health} {status.ErrorMessage}");
                }

                int count = 0;

                foreach (WorkloadInstanceStatus status in nonSysStatusList)
                {
                    Console.WriteLine($"{configuredWorkloads[count++].Name} Id : {configuredWorkloads[count++].WorkloadInstanceId} Workload installation status: {status.Health} {status.ErrorMessage}");
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
