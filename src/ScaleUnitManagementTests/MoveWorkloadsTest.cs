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

        [TestInitialize]
        public void Setup()
        {
            Initialize();
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
                workloadMover.SetScaleUnitAosClient(aosClient.Object);
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
