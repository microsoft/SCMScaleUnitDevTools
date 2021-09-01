using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using ScaleUnitManagement.Utilities;
using Moq;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;
using CloudAndEdgeLibs.AOS;
using System;

namespace ScaleUnitManagementTests
{
    [TestClass]
    public sealed class WorkloadUpgradeTest : DevToolsUnitTest
    {
        private Mock<IAOSClient> hubAosClient;
        private WorkloadInstanceStatus workloadStatus;
        private List<WorkloadInstance> workloadInstances;
        private List<Workload> workloads;

        [TestInitialize]
        public void Setup()
        {
            Initialize();

            hubAosClient = new Mock<IAOSClient>();
            workloadInstances = new List<WorkloadInstance> { exampleWorkload };
            workloadStatus = new WorkloadInstanceStatus();
            workloads = new List<Workload> { exampleWorkload.VersionedWorkload.Workload };

            hubAosClient.Setup(x => x.GetWorkloads())
                .ReturnsAsync(workloads);

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(workloadInstances));

            aosClient.Setup(x => x.WriteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((newWorkloadInstances) =>
                {
                    workloadInstances = newWorkloadInstances;
                })
                .Returns(() => Task.FromResult(workloadInstances));

            aosClient.Setup(x => x.CheckWorkloadStatus(It.IsAny<string>()))
                .ReturnsAsync(workloadStatus);
        }

        [TestMethod]
        public async Task UpgradeWorkloads_WithStatusStopped_UpdatesId()
        {
            // Arrange
            workloadStatus.Health = "Stopped";
            var oldVersionedWorkloadId = exampleWorkload.VersionedWorkload.Id;

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetScaleUnitAosClient(aosClient.Object);
                workloadDefinitionManager.SetHubAosClient(hubAosClient.Object);
                await workloadDefinitionManager.UpgradeWorkloadsDefinition();
            }

            // Assert
            var newVersionedWorkloadId = workloadInstances.First().VersionedWorkload.Id;
            newVersionedWorkloadId.Should().NotBe(oldVersionedWorkloadId);
        }

        [TestMethod]
        public async Task UpgradeWorkloads_WithStatusRunning_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Running";
            bool exceptionThrown = false;

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetScaleUnitAosClient(aosClient.Object);
                workloadDefinitionManager.SetHubAosClient(hubAosClient.Object);

                try
                {
                    await workloadDefinitionManager.UpgradeWorkloadsDefinition();
                }
                catch (Exception ex)
                {
                    exceptionThrown = true;
                }
            }

            // Assert
            exceptionThrown.Should().BeTrue();
        }

        [TestMethod]
        public async Task UpgradeWorkloads_WorkloadDoesNotExist_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Stopped";
            bool exceptionThrown = false;
            workloads.Clear();

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetScaleUnitAosClient(aosClient.Object);
                workloadDefinitionManager.SetHubAosClient(hubAosClient.Object);

                try
                {
                    await workloadDefinitionManager.UpgradeWorkloadsDefinition();
                }
                catch (Exception ex)
                {
                    exceptionThrown = true;
                }
            }

            // Assert
            exceptionThrown.Should().BeTrue();
        }
    }
}
