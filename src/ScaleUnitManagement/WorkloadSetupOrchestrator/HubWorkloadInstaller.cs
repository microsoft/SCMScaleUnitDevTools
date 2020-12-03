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
                WorkloadInstanceStatus sysStatus = await hubAosClient.CheckWorkloadStatus(Config.SysWorkloadInstanceId);
                WorkloadInstanceStatus mesStatus = await hubAosClient.CheckWorkloadStatus(Config.MesWorkloadInstanceId);
                WorkloadInstanceStatus wesStatus = await hubAosClient.CheckWorkloadStatus(Config.WesWorkloadInstanceId);

                Console.WriteLine($"SYS workload installation status: {sysStatus.Health} {sysStatus.ErrorMessage}");
                Console.WriteLine($"MES workload installation status: {mesStatus.Health} {mesStatus.ErrorMessage}");
                Console.WriteLine($"WES workload installation status: {wesStatus.Health} {wesStatus.ErrorMessage}");
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
            var sysWorkloadInstance = expectedWorkloadInstances.Find((instance) => instance.Id.Equals(Config.SysWorkloadInstanceId));
            var wesWorkloadInstance = expectedWorkloadInstances.Find((instance) => instance.Id.Equals(Config.WesWorkloadInstanceId));
            var mesWorkloadInstance = expectedWorkloadInstances.Find((instance) => instance.Id.Equals(Config.MesWorkloadInstanceId));

            var foundSysWorkloadInstance = createdInstances.Find((instance) => instance.Id.Equals(Config.SysWorkloadInstanceId));
            var foundMESWorkloadInstance = createdInstances.Find((instance) => instance.Id.Equals(Config.MesWorkloadInstanceId));
            var foundWESWorkloadInstance = createdInstances.Find((instance) => instance.Id.Equals(Config.WesWorkloadInstanceId));

            createdInstances.Should().NotBeNull();
            foundSysWorkloadInstance.Should().NotBeNull();
            foundMESWorkloadInstance.Should().NotBeNull();
            foundWESWorkloadInstance.Should().NotBeNull();

            foundSysWorkloadInstance.WithDeepEqual(sysWorkloadInstance).Assert();
            foundMESWorkloadInstance.WithDeepEqual(mesWorkloadInstance).Assert();
            foundWESWorkloadInstance.WithDeepEqual(wesWorkloadInstance).Assert();
        }

        private async Task WaitForWorkloadInstanceReadinessOnHub()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                WorkloadInstanceStatus sysStatus = await hubAosClient.CheckWorkloadStatus(Config.SysWorkloadInstanceId);
                WorkloadInstanceStatus mesStatus = await hubAosClient.CheckWorkloadStatus(Config.MesWorkloadInstanceId);
                WorkloadInstanceStatus wesStatus = await hubAosClient.CheckWorkloadStatus(Config.WesWorkloadInstanceId);

                sysStatus.Should().NotBeNull("SYS workload instance should be found");
                mesStatus.Should().NotBeNull("MES workload instance should be found");
                wesStatus.Should().NotBeNull("WES workload instance should be found");

                sysStatus.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
                mesStatus.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
                wesStatus.Health.Should().Be(WorkloadInstanceHealthConstants.Running);
            }, "Wait for workload instance readiness");
        }
    }
}
