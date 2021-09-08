using System.Collections.Generic;
using CloudAndEdgeLibs.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;
using FluentAssertions;
using System;

namespace ScaleUnitManagementTests
{
    [TestClass]
    public class WorkloadInstanceTopologicalSortUtilTest
    {
        private readonly string sys = "Sys";
        private readonly string mes = "Mes";
        private readonly string wes = "Wes";

        [TestMethod]
        public void New_NullWorkloadInstanceList_ExceptionThrown()
        {
            // Arrange
            var exceptionThrown = false;
            List<WorkloadInstance> workloadInstances = null;

            // Act
            try
            {
                var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            }
            catch (ArgumentNullException)
            {
                exceptionThrown = true;
            }

            exceptionThrown.Should().BeTrue();
        }

        [TestMethod]
        public void Sort_EmptyWorkloadInstanceList_EmptyListReturned()
        {
            // Arrange
            var workloadInstances = new List<WorkloadInstance>();

            // Act
            var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            var sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances.Should().HaveCount(0);
        }

        [TestMethod]
        public void Sort_ThreeWorkloadInstancesInput_SortedListHasAllWorkloadInstances()
        {
            // Arrange
            var w1 = BuildWorkloadInstance(sys);
            var w2 = BuildWorkloadInstance(mes);
            var w3 = BuildWorkloadInstance(wes);

            var workloadInstances = new List<WorkloadInstance>() { w1, w2, w3 };

            // Act
            var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            var sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances.Should().HaveCount(workloadInstances.Count);
            foreach (var expectedWorkloadInstance in workloadInstances)
            {
                sortedWorkloadInstances.Should().Contain(expectedWorkloadInstance);
            }
        }

        [TestMethod]
        public void Sort_MesWesDependOnSys_SysBeforeMesWes()
        {
            // Arrange
            var w1 = BuildWorkloadInstance(sys);

            var mesDependencies = new List<string> { sys };
            var w2 = BuildWorkloadInstance(mes, mesDependencies);

            var wesDependencies = new List<string> { sys };
            var w3 = BuildWorkloadInstance(wes, wesDependencies);

            var workloadInstances = new List<WorkloadInstance>() { w2, w3, w1 };

            // Act
            var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            var sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances[0].Should().Be(w1);
        }

        [TestMethod]
        public void Sort_MesDependsOnWesAndSysWesDependsOnSys_ResultSysWesMes()
        {
            // Arrange
            var w1 = BuildWorkloadInstance(sys);

            var mesDependencies = new List<string> { sys, wes };
            var w2 = BuildWorkloadInstance(mes, mesDependencies);

            var wesDependencies = new List<string> { sys };
            var w3 = BuildWorkloadInstance(wes, wesDependencies);

            var workloadInstances = new List<WorkloadInstance>() { w2, w3, w1 };

            // Act
            var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            var sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances[0].Should().Be(w1);
            sortedWorkloadInstances[1].Should().Be(w3);
        }

        [TestMethod]
        public void Sort_FourElementsWithMultipleDependencies_CorrectSort()
        {
            const string TEST = "Test";

            var sysDependencies = new List<string> { mes };
            var w1 = BuildWorkloadInstance(sys, sysDependencies);

            var mesDependencies = new List<string> { TEST };
            var w2 = BuildWorkloadInstance(mes, mesDependencies);

            var wesDependencies = new List<string> { sys, TEST };
            var w3 = BuildWorkloadInstance(wes, wesDependencies);

            var w4 = BuildWorkloadInstance(TEST);

            var workloadInstances = new List<WorkloadInstance>() { w1, w2, w3, w4 };

            // Act
            var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            var sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            var expectedSortedList = new List<WorkloadInstance> { w4, w2, w1, w3 };

            for (var i = 0; i < sortedWorkloadInstances.Count; i++)
            {
                sortedWorkloadInstances[i].Should().Be(expectedSortedList[i]);
            }
        }

        private WorkloadInstance BuildWorkloadInstance(string name, List<string> dependsOn = null)
        {
            return new WorkloadInstance()
            {
                Id = Guid.NewGuid().ToString(),
                VersionedWorkload = new VersionedWorkload()
                {
                    Workload = new Workload()
                    {
                        Name = name,
                        DependsOn = dependsOn ?? new List<string>()
                    }
                }
            };
        }

    }
}
