using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using ScaleUnitManagement.Utilities;
using Moq;
using CloudAndEdgeLibs.AOS;

namespace ScaleUnitManagementTests
{

    [TestClass]
    public sealed class DrainWorkloadsTest : DevToolsUnitTest
    {
        private WorkloadInstanceStatus workloadStatus;

        [TestInitialize]
        public void Setup()
        {
            workloadStatus = new WorkloadInstanceStatus();

            aosClient.Setup(x => x.DrainWorkload(It.IsAny<string>()))
            .Callback<string>((workloadId) =>
            {
                if (!workloadStatus.Health.Equals("Running"))
                {
                    throw new Exception("Workload not running");
                }
                if (workloadInstances.Exists(workload => workload.Id.Equals(workloadId)))
                {
                    workloadStatus.Health = "Stopped";
                }
                else
                {
                    throw new Exception("Workload does not exist");
                }
            });

            aosClient.Setup(x => x.CheckWorkloadStatus(It.IsAny<string>()))
            .ReturnsAsync(workloadStatus);
        }

        [TestMethod]
        public async Task DrainWorkloads_WithStatusRunning_DrainsWorkload()
        {
            // Arrange
            workloadStatus.Health = "Running";

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetScaleUnitAosClient(aosClient.Object);
                await pipelineManager.DrainWorkloadDataPipelines();
            }

            // Assert
            workloadStatus.Health.Should().Be("Stopped");
        }

        [TestMethod]
        public async Task DrainWorkloads_WithStatusStopped_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Stopped";

            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetScaleUnitAosClient(aosClient.Object);
                Func<Task> act = async () => await pipelineManager.DrainWorkloadDataPipelines();

                // Act + Assert
                await act.Should().ThrowAsync<Exception>(because: "Workload is not running");
            }
        }

        [TestMethod]
        public async Task DrainWorkloads_WorkloadDoesNotExist_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Running";
            WorkloadInstance corruptedWorkloadInstance = new ConfigurationHelper().GetExampleWorkload();
            corruptedWorkloadInstance.Id = "scrambled id";
            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance> { corruptedWorkloadInstance });

            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetScaleUnitAosClient(aosClient.Object);
                Func<Task> act = async () => await pipelineManager.DrainWorkloadDataPipelines();

                // Act + Assert
                await act.Should().ThrowAsync<Exception>(because: "Workload does not exist");
            }
        }
    }
}
