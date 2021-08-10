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

            Config.UserConfigImplementation = loadConfigMock;
        }

        [TestMethod]
        public async Task DeleteWorkloads()
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

            aosClient.Setup(x => x.DeleteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((deletedWorkloads) =>
                {
                    toBeReturnedWorkloadInstances = toBeReturnedWorkloadInstances
                                                        .Where(w1 => !deletedWorkloads.Any( w2 => w1.Id == w2.Id)).ToList();
                })
                .Returns(() => Task.FromResult(toBeReturnedWorkloadInstances));

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                WorkloadDeleter workloadDeleter = new WorkloadDeleter();
                workloadDeleter.SetClient(aosClient.Object);
                await workloadDeleter.DeleteWorkloadsFromScaleUnit();
            }

            // Assert
            toBeReturnedWorkloadInstances.Should().BeEmpty();
        }
    }
}
