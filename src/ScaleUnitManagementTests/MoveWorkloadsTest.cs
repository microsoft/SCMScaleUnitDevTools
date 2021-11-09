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

namespace ScaleUnitManagementTests
{
    [TestClass]
    public sealed class MoveWorkloadsTest : DevToolsUnitTest
    {
        [TestMethod]
        public async Task MoveWorkloads()
        {
            // Arrange
            aosClient.Setup(x => x.WriteWorkloadInstances(It.IsAny<List<WorkloadInstance>>()))
                .Callback<List<WorkloadInstance>>((workloads) =>
                {
                    workloadInstances = workloads;
                })
                .Returns(() => Task.FromResult(workloadInstances));

            // Act
            using (ScaleUnitContext.CreateContext(scaleUnitId))
            {
                WorkloadMover workloadMover = new WorkloadMover();
                workloadMover.SetScaleUnitAosClient(aosClient.Object);
                await workloadMover.MoveWorkloads(hubId);
            }

            // Assert
            workloadInstances.Should().NotBeEmpty();
            foreach (var workload in workloadInstances)
            {
                TemporalAssignment temporalAssignment = workload.ExecutingEnvironment.Last();
                temporalAssignment.Environment.ScaleUnitId.Should().Be(hubId);
            }
        }
    }
}
