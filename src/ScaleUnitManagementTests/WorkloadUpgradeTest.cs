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

namespace ScaleUnitManagementTests
{

    [TestClass]
    public sealed class WorkloadUpgradeTest
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
        public async Task UpgradeWorkloads()
        {
            // Arrange
            var workloadInstances = new List<WorkloadInstance> { exampleWorkload };
            var workloads = new List<Workload> { exampleWorkload.VersionedWorkload.Workload };
            var oldWorkloadInstanceId = exampleWorkload.VersionedWorkload.Id;

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(workloadInstances));

            aosClient.Setup(x => x.GetWorkloads())
                .ReturnsAsync(new List<Workload>(workloads));

            aosClient.Setup(x => x.WriteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((newWorkloadInstances) =>
                {
                    workloadInstances = newWorkloadInstances;
                })
                .Returns(() => Task.FromResult(workloadInstances));

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                workloadDefinitionManager.SetClient(aosClient.Object);
                await workloadDefinitionManager.UpgradeWorkloadsDefinition();
            }

            // Assert
            var newWorkloadInstanceId = workloadInstances.First().VersionedWorkload.Id;
            newWorkloadInstanceId.Should().NotBe(oldWorkloadInstanceId);
        }
    }
}
