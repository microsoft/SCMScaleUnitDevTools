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
    public sealed class WorkloadMovementTest
    {
        private Mock<IAOSClient> aosClient;
        private readonly string scaleUnitId = "@A";
        private readonly string hubId = "@@";
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
        public async Task MoveWorkloads()
        {
            // Arrange
            var toBeReturnedWorkloadInstances = new List<WorkloadInstance>() { exampleWorkload };

            aosClient.Setup(x => x.GetWorkloadInstances())
                .Returns(() => Task.FromResult(new List<WorkloadInstance>(toBeReturnedWorkloadInstances)));

            aosClient.Setup(x => x.WriteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((workloads) =>
                {
                    toBeReturnedWorkloadInstances = workloads;
                })
                .Returns(() => Task.FromResult(toBeReturnedWorkloadInstances));

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                WorkloadMover workloadMover = new WorkloadMover();
                workloadMover.SetClient(aosClient.Object);
                await workloadMover.MoveWorkloads(hubId, DateTime.UtcNow);
            }

            // Assert
            toBeReturnedWorkloadInstances.Should().NotBeEmpty();
            foreach (var workload in toBeReturnedWorkloadInstances)
            {
                TemporalAssignment temporalAssignment = workload.ExecutingEnvironment.Last();
                temporalAssignment.Environment.ScaleUnitId.Should().Be(hubId);
            }
        }
    }
}
