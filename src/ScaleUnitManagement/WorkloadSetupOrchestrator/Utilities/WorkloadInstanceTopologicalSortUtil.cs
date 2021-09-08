using System;
using System.Collections.Generic;
using System.Linq;
using CloudAndEdgeLibs.Contracts;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    /// <summary>
    /// The <c>WorkloadInstanceTopologicalSortUtil</c> provides the necessary funcitonality that
    /// allows a list of workload instances to be sorted based on the DependsOn field of the enclosed
    /// workload, so we can install workloads in the right order and ensure that any depended workloads
    /// are already installed.
    /// </summary>
    public class WorkloadInstanceTopologicalSortUtil
    {
        private readonly HashSet<WorkloadInstanceDFSNode> nonProcessedNodes;
        private readonly Dictionary<string, List<WorkloadInstanceDFSNode>> nameToDFSNodesMap;
        private readonly List<WorkloadInstance> sortedWorkloadInstanceList;

        public WorkloadInstanceTopologicalSortUtil(List<WorkloadInstance> workloadInstances)
        {
            if (workloadInstances == null)
                throw new ArgumentNullException();

            nonProcessedNodes = BuildNonProcessedNodes(workloadInstances);
            nameToDFSNodesMap = BuildNameToDFSNodesMap(nonProcessedNodes);
            sortedWorkloadInstanceList = new List<WorkloadInstance>();
        }

        public List<WorkloadInstance> Sort()
        {
            while (nonProcessedNodes.Count > 0)
            {
                ProcessNode(nonProcessedNodes.First());
            }

            return sortedWorkloadInstanceList;
        }

        private void ProcessNode(WorkloadInstanceDFSNode node)
        {
            if (!nonProcessedNodes.Contains(node))
                return;

            if (node.InProgress)
                throw new Exception("Cyclic dependency detected between workloads.");

            node.InProgress = true;

            foreach (var dependedNode in GetDependedNodes(node))
                ProcessNode(dependedNode);

            node.InProgress = false;
            nonProcessedNodes.Remove(node);
            sortedWorkloadInstanceList.Add(node.WorkloadInstance);
        }

        private IEnumerable<WorkloadInstanceDFSNode> GetDependedNodes(WorkloadInstanceDFSNode node)
        {
            var dependedNodes = new List<WorkloadInstanceDFSNode>();

            foreach (var workloadName in node.WorkloadInstance.VersionedWorkload.Workload.DependsOn)
            {
                dependedNodes = dependedNodes.Concat(nameToDFSNodesMap[workloadName]).ToList();
            }

            return dependedNodes;
        }

        class WorkloadInstanceDFSNode
        {
            public WorkloadInstance WorkloadInstance { get; set; }
            public bool InProgress { get; set; }

            public WorkloadInstanceDFSNode(WorkloadInstance workloadInstance)
            {
                this.WorkloadInstance = workloadInstance;
                this.InProgress = false;
            }
        }

        private HashSet<WorkloadInstanceDFSNode> BuildNonProcessedNodes(List<WorkloadInstance> workloadInstances)
        {
            var nonProcessedNodes = new HashSet<WorkloadInstanceDFSNode>();

            foreach (var workloadInstance in workloadInstances)
            {
                nonProcessedNodes.Add(new WorkloadInstanceDFSNode(workloadInstance));
            }

            return nonProcessedNodes;
        }

        private Dictionary<string, List<WorkloadInstanceDFSNode>> BuildNameToDFSNodesMap(HashSet<WorkloadInstanceDFSNode> dfsNodes)
        {
            var nameToDfsNodesMap = new Dictionary<string, List<WorkloadInstanceDFSNode>>();

            foreach (var node in dfsNodes)
            {
                if (!nameToDfsNodesMap.TryGetValue(node.WorkloadInstance.VersionedWorkload.Workload.Name, out var nodesForName))
                    nodesForName = new List<WorkloadInstanceDFSNode>();

                nodesForName.Add(node);
                nameToDfsNodesMap[node.WorkloadInstance.VersionedWorkload.Workload.Name] = nodesForName;
            }

            return nameToDfsNodesMap;
        }
    }
}
