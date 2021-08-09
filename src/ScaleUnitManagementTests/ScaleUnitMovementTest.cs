using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using ScaleUnitManagement.Utilities;
using Microsoft.QualityTools.Testing.Fakes;
using ShimAOSClient = ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities.Fakes.ShimAOSClient;

namespace ScaleUnitManagementTests
{

    [TestClass]
    public sealed class ScaleUnitMovementTest
    {

        [TestMethod]
        public async Task MoveWorkloads()
        {
            using (ShimsContext.Create())
            {
                // Arrange
                var scaleUnitId = "@A";
                var hubId = Config.HubScaleUnit().ScaleUnitId;
                var toBeReturnedWorkloadInstances = new List<WorkloadInstance>();
                WorkloadInstance workloadInstance = new WorkloadInstance { Id = Guid.NewGuid().ToString() };
                workloadInstance.ExecutingEnvironment.Add(new TemporalAssignment
                {
                    EffectiveDate = DateTime.UtcNow,
                    Environment = new PhysicalEnvironmentReference() { ScaleUnitId = scaleUnitId },
                });

                toBeReturnedWorkloadInstances.Add(new WorkloadInstance { Id = Guid.NewGuid().ToString() });

                ShimAOSClient.ConstructorHttpClientString = (@this, client, namestring) =>
                {
                    var shim = new ShimAOSClient(@this) { };
                };

                ShimAOSClient.AllInstances.GetWorkloadInstances = async (@this) => toBeReturnedWorkloadInstances;
                ShimAOSClient.AllInstances.WriteWorkloadInstancesListOfWorkloadInstance = async (@this, workloads) =>
                {
                    toBeReturnedWorkloadInstances = workloads;
                    return workloads;
                };


                // Act
                using (ScaleUnitContext.CreateContext(scaleUnitId))
                {
                    WorkloadMover workloadMover = new WorkloadMover();
                    await workloadMover.MoveWorkloads(hubId, DateTime.UtcNow);
                }

                // Assert
                foreach(WorkloadInstance workload in toBeReturnedWorkloadInstances)
                {
                    TemporalAssignment temporalAssignment = workload.ExecutingEnvironment.Last();
                    temporalAssignment.Environment.ScaleUnitId.Should().Be(hubId);
                }

            }
        }

    }
}
