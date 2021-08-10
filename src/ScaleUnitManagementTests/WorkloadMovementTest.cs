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

        [TestInitialize]
        public void Setup()
        {
            aosClient = new Mock<IAOSClient>();

            Func<CloudAndEdgeConfiguration> loadConfigMock = () =>
            {
                ConfigurationHelper configurationHelper = new ConfigurationHelper();
                return configurationHelper.ConstructTestConfiguration();
            };

            Config.UserConfigImplementation = loadConfigMock;
        }

        [TestMethod]
        public async Task MoveWorkloads()
        {
            // Arrange
            var toBeReturnedWorkloadInstances = new List<WorkloadInstance>();

            ConfiguredWorkload configuredWorkload = Config.WorkloadList().First();
            WorkloadInstance workloadInstance = new WorkloadInstance { Id = configuredWorkload.WorkloadInstanceId };
            workloadInstance.ExecutingEnvironment.Add(new TemporalAssignment
            {
                EffectiveDate = DateTime.UtcNow,
                Environment = new PhysicalEnvironmentReference() { ScaleUnitId = scaleUnitId },
            });
            toBeReturnedWorkloadInstances.Add(workloadInstance);

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
            foreach(WorkloadInstance workload in toBeReturnedWorkloadInstances)
            {
                TemporalAssignment temporalAssignment = workload.ExecutingEnvironment.Last();
                temporalAssignment.Environment.ScaleUnitId.Should().Be(hubId);
            }
        }
    }
}
