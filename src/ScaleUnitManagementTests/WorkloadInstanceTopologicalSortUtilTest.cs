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
        private readonly string SYS = "Sys";
        private readonly string MES = "Mes";
        private readonly string WES = "Wes";

        [TestMethod]
        public void New_NullWorkloadInstanceList_ExceptionThrown()
        {
            // Arrange
            bool exceptionThrown = false;
            List<WorkloadInstance> workloadInstances = null;

            // Act
            try
            {
                WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
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
            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>();

            // Act
            WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            List<WorkloadInstance> sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances.Should().HaveCount(0);
        }

        [TestMethod]
        public void Sort_ThreeWorkloadInstancesInput_SortedListHasAllWorkloadInstances()
        {
            // Arrange
            WorkloadInstance w1 = BuildWorkloadInstance(SYS);
            WorkloadInstance w2 = BuildWorkloadInstance(MES);
            WorkloadInstance w3 = BuildWorkloadInstance(WES);

            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>() { w1, w2, w3 };

            // Act
            WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            List<WorkloadInstance> sortedWorkloadInstances = topologicalSortUtil.Sort();

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
            WorkloadInstance w1 = BuildWorkloadInstance(SYS);

            List<string> mesDependencies = new List<string> { SYS };
            WorkloadInstance w2 = BuildWorkloadInstance(MES, mesDependencies);

            List<string> wesDependencies = new List<string> { SYS };
            WorkloadInstance w3 = BuildWorkloadInstance(WES, wesDependencies);

            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>() { w2, w3, w1 };

            // Act
            WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            List<WorkloadInstance> sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances[0].Should().Be(w1);
        }

        [TestMethod]
        public void Sort_MesDependsOnWesAndSysWesDependsOnSys_ResultSysWesMes()
        {
            // Arrange
            WorkloadInstance w1 = BuildWorkloadInstance(SYS);

            List<string> mesDependencies = new List<string> { SYS, WES };
            WorkloadInstance w2 = BuildWorkloadInstance(MES, mesDependencies);

            List<string> wesDependencies = new List<string> { SYS };
            WorkloadInstance w3 = BuildWorkloadInstance(WES, wesDependencies);

            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>() { w2, w3, w1 };

            // Act
            WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            List<WorkloadInstance> sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            sortedWorkloadInstances[0].Should().Be(w1);
            sortedWorkloadInstances[1].Should().Be(w3);
        }

        [TestMethod]
        public void Sort_FourElementsWithMultipleDependencies_CorrectSort()
        {
            const string TEST = "Test";

            List<string> sysDependencies = new List<string> { MES };
            WorkloadInstance w1 = BuildWorkloadInstance(SYS, sysDependencies);

            List<string> mesDependencies = new List<string> { TEST };
            WorkloadInstance w2 = BuildWorkloadInstance(MES, mesDependencies);

            List<string> wesDependencies = new List<string> { SYS, TEST };
            WorkloadInstance w3 = BuildWorkloadInstance(WES, wesDependencies);

            WorkloadInstance w4 = BuildWorkloadInstance(TEST);

            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>() { w1, w2, w3, w4 };

            // Act
            WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(workloadInstances);
            List<WorkloadInstance> sortedWorkloadInstances = topologicalSortUtil.Sort();

            // Assert
            List<WorkloadInstance> expectedSortedList = new List<WorkloadInstance> { w4, w2, w1, w3 };

            for (int i = 0; i < sortedWorkloadInstances.Count; i++)
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
                        DependsOn = dependsOn == null ? new List<string>() : dependsOn
                    }
                }
            };
        }

    }
}
