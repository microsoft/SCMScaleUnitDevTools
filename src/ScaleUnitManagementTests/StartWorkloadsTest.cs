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
    public sealed class StartWorkloadsTest : DevToolsUnitTest
    {
        private WorkloadInstanceStatus workloadStatus;

        [TestInitialize]
        public void Setup()
        {
            Initialize();

            workloadStatus = new WorkloadInstanceStatus();
            var workloadInstances = new List<WorkloadInstance> { exampleWorkload };

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(workloadInstances));

            aosClient.Setup(x => x.StartWorkload(It.IsAny<string>()))
            .Callback<string>((workloadId) =>
            {
                if (!workloadStatus.Health.Equals("Stopped"))
                {
                    throw new Exception("Workload not stopped");
                }
                if (workloadInstances.Exists(workload => workload.Id.Equals(workloadId)))
                {
                    workloadStatus.Health = "Running";
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
        public async Task StartWorkloads_WithStatusStopped_StartsWorkload()
        {
            // Arrange
            workloadStatus.Health = "Stopped";

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetScaleUnitAosClient(aosClient.Object);
                await pipelineManager.StartWorkloadDataPipelines();
            }

            // Assert
            workloadStatus.Health.Should().Be("Running");
        }

        [TestMethod]
        public async Task StartWorkloads_WithStatusRunning_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Running";
            var exception = "";

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetScaleUnitAosClient(aosClient.Object);
                try
                {
                    await pipelineManager.StartWorkloadDataPipelines();
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
            }

            // Assert
            exception.Should().Be("Workload not stopped");
        }

        [TestMethod]
        public async Task StartWorkloads_WorkloadDoesNotExist_ThrowsException()
        {
            // Arrange
            workloadStatus.Health = "Stopped";
            var exception = "";
            var corruptedWorkloadInstance = new ConfigurationHelper().GetExampleWorkload();
            corruptedWorkloadInstance.Id = "scrambled id";
            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance> { corruptedWorkloadInstance });

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetScaleUnitAosClient(aosClient.Object);
                try
                {
                    await pipelineManager.StartWorkloadDataPipelines();
                }
                catch (Exception ex)
                {
                    exception = ex.Message;
                }
            }

            // Assert
            exception.Should().Be("Workload does not exist");
        }
    }
}
