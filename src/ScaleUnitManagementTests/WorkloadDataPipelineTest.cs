using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using ScaleUnitManagement.Utilities;
using Moq;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;
using CloudAndEdgeLibs.AOS;

namespace ScaleUnitManagementTests
{

    [TestClass]
    public sealed class WorkloadDataaPipelineTest
    {
        private Mock<IAOSClient> aosClient;
        private readonly string scaleUnitId = "@A";
        private WorkloadInstance exampleWorkload;

        [TestInitialize]
        public void Setup()
        {
            aosClient = new Mock<IAOSClient>();

            ConfigurationHelper configurationHelper = new ConfigurationHelper();

            Func<CloudAndEdgeConfiguration> loadConfigMock = () =>
            {
                return configurationHelper.GetTestConfiguration();
            };

            Config.GetUserConfigImplementation = loadConfigMock;

            exampleWorkload = configurationHelper.GetExampleWorkload();
        }

        [TestMethod]
        public async Task DrainWorkloads()
        {
            // Arrange
            var status = new WorkloadInstanceStatus() { Health = "Running" };
            var workloadInstances = new List<WorkloadInstance> { exampleWorkload };

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(workloadInstances));

            aosClient.Setup(x => x.DrainWorkload(It.IsAny<string>()))
                .Callback<string>((workloadId) =>
                {
                    status.Health = "Stopped";
                });

            aosClient.Setup(x => x.CheckWorkloadStatus(It.IsAny<string>()))
            .ReturnsAsync(status);

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetClient(aosClient.Object);
                await pipelineManager.DrainWorkloadDataPipelines();
            }

            // Assert
            status.Health.Should().Be("Stopped");
        }

        [TestMethod]
        public async Task StartWorkloads()
        {
            // Arrange
            var status = new WorkloadInstanceStatus() { Health = "Stopped" };
            var workloadInstances = new List<WorkloadInstance> { exampleWorkload };

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(workloadInstances));

            aosClient.Setup(x => x.StartWorkload(It.IsAny<string>()))
                .Callback<string>((workloadId) =>
                {
                    status.Health = "Running";
                });

            aosClient.Setup(x => x.CheckWorkloadStatus(It.IsAny<string>()))
            .ReturnsAsync(status);

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var pipelineManager = new PipelineManager();
                pipelineManager.SetClient(aosClient.Object);
                await pipelineManager.StartWorkloadDataPipelines();
            }

            // Assert
            status.Health.Should().Be("Running");
        }
    }
}
