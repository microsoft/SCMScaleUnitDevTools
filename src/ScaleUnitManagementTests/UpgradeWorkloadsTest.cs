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
        private List<Workload> workloads;

        [TestInitialize]
        public void Setup()
        {
            hubAosClient = new Mock<IAOSClient>();
            workloadStatus = new WorkloadInstanceStatus();
            workloads = new List<Workload> { exampleWorkload.VersionedWorkload.Workload };

            hubAosClient.Setup(x => x.GetWorkloads())
                .ReturnsAsync(workloads);

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
            string oldVersionedWorkloadId = exampleWorkload.VersionedWorkload.Id;

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetScaleUnitAosClient(aosClient.Object);
                workloadDefinitionManager.SetHubAosClient(hubAosClient.Object);
                await workloadDefinitionManager.UpgradeWorkloadsDefinition();
            }

            // Assert
            string newVersionedWorkloadId = workloadInstances.First().VersionedWorkload.Id;
            newVersionedWorkloadId.Should().NotBe(oldVersionedWorkloadId);
        }

        [TestMethod]
        public async Task UpgradeWorkloads_WithStatusRunning_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Running";

            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetScaleUnitAosClient(aosClient.Object);
                workloadDefinitionManager.SetHubAosClient(hubAosClient.Object);
                Func<Task> act = async () => await workloadDefinitionManager.UpgradeWorkloadsDefinition();

                // Act + Assert
                await act.Should().ThrowAsync<Exception>(because: "Workload is not drained");
            }
        }

        [TestMethod]
        public async Task UpgradeWorkloads_WorkloadDoesNotExist_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Stopped";
            workloads.Clear();

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetScaleUnitAosClient(aosClient.Object);
                workloadDefinitionManager.SetHubAosClient(hubAosClient.Object);
                Func<Task> act = async () => await workloadDefinitionManager.UpgradeWorkloadsDefinition();

                // Act + Assert
                await act.Should().ThrowAsync<Exception>(because: "Workload does not exist");
            }
        }
    }
}
