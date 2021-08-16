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
    public sealed class WorkloadDeletionTest
    {
        private Mock<IAOSClient> aosClient;
        private readonly string scaleUnitId = "@A";

        [TestInitialize]
        public void Setup()
        {
            aosClient = new Mock<IAOSClient>();

            Func<CloudAndEdgeConfiguration> loadConfigMock = () =>
            {
                ConfigurationHelper configurationHelper = new ConfigurationHelper();
                return configurationHelper.ConstructTestConfiguration();
            };

            Config.GetUserConfigImplementation = loadConfigMock;
        }

        [TestMethod]
        public async Task DeleteWorkloads()
        {
            // Arrange
            var configuredWorkload = Config.WorkloadList().First();
            var workloadInstance = new WorkloadInstance
            {
                Id = configuredWorkload.WorkloadInstanceId,
                VersionedWorkload = new VersionedWorkload
                {
                    Workload = new Workload { Name = configuredWorkload.Name },
                },
            };
            workloadInstance.ExecutingEnvironment.Add(new TemporalAssignment
            {
                EffectiveDate = DateTime.UtcNow,
                Environment = new PhysicalEnvironmentReference() { ScaleUnitId = scaleUnitId },
            });

            var toBeReturnedWorkloadInstances = new List<WorkloadInstance> { workloadInstance };

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(toBeReturnedWorkloadInstances));

            aosClient.Setup(x => x.DeleteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((deletedWorkloads) =>
                {
                    toBeReturnedWorkloadInstances = toBeReturnedWorkloadInstances
                                                        .Where(w1 => !deletedWorkloads.Any(w2 => w1.Id.Equals(w2.Id))).ToList();
                })
                .ReturnsAsync(toBeReturnedWorkloadInstances);

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDeleter = new WorkloadDeleter();
                workloadDeleter.SetClient(aosClient.Object);
                await workloadDeleter.DeleteWorkloadsFromScaleUnit();
            }

            // Assert
            toBeReturnedWorkloadInstances.Should().BeEmpty();
        }
    }
}
