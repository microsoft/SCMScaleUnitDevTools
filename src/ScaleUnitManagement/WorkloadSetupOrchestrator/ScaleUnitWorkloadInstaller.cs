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
                WorkloadInstanceStatus sysStatus = await scaleUnitAosClient.CheckWorkloadStatus(Config.SysWorkloadInstanceId);
                WorkloadInstanceStatus mesStatus = await scaleUnitAosClient.CheckWorkloadStatus(Config.MesWorkloadInstanceId);
                WorkloadInstanceStatus wesStatus = await scaleUnitAosClient.CheckWorkloadStatus(Config.WesWorkloadInstanceId);

                Console.WriteLine($"SYS workload installation status: {sysStatus.Health} {sysStatus.ErrorMessage}");
                Console.WriteLine($"MES workload installation status: {mesStatus.Health} {mesStatus.ErrorMessage}");
                Console.WriteLine($"WES workload installation status: {wesStatus.Health} {wesStatus.ErrorMessage}");
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
    }
}
