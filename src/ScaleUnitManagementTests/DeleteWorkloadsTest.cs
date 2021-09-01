using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using ScaleUnitManagement.Utilities;
using Moq;

namespace ScaleUnitManagementTests
{

    [TestClass]
    public sealed class DeleteWorkloadsTest : DevToolsUnitTest
    {

        [TestInitialize]
        public void Setup()
        {
            Initialize();
        }

        [TestMethod]
        public async Task DeleteWorkloads()
        {
            // Arrange
            var toBeReturnedWorkloadInstances = new List<WorkloadInstance> { exampleWorkload };

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
                workloadDeleter.SetScaleUnitAosClient(aosClient.Object);
                await workloadDeleter.DeleteWorkloadsFromScaleUnit();
            }

            // Assert
            toBeReturnedWorkloadInstances.Should().BeEmpty();
        }
    }
}
