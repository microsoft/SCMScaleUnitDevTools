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
        [TestMethod]
        public async Task DeleteWorkloads()
        {
            // Arrange
            aosClient.Setup(x => x.DeleteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((deletedWorkloads) =>
                {
                    workloadInstances = workloadInstances.Where(w1 => !deletedWorkloads.Any(w2 => w1.Id.Equals(w2.Id))).ToList();
                })
                .ReturnsAsync(workloadInstances);

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                var workloadDeleter = new WorkloadDeleter();
                workloadDeleter.SetScaleUnitAosClient(aosClient.Object);
                await workloadDeleter.DeleteWorkloadsFromScaleUnit();
            }

            // Assert
            workloadInstances.Should().BeEmpty();
        }
    }
}
