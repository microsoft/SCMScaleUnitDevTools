using System;
using System.Collections.Generic;
using System.Linq;
using CloudAndEdgeLibs.Contracts;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    class WorkloadInstanceTopologicalSortUtil
    {
        private readonly HashSet<WorkloadInstanceDFSNode> nonProcessedNodes;
        private readonly Dictionary<string, List<WorkloadInstanceDFSNode>> nameToDFSNodesMap;
        private readonly List<WorkloadInstance> sortedWorkloadInstanceList;

        public WorkloadInstanceTopologicalSortUtil(List<WorkloadInstance> workloadInstances)
        {
            nonProcessedNodes = BuildNonProcessedNodes(workloadInstances);
            nameToDFSNodesMap = BuildNameToDFSNodesMap(nonProcessedNodes);
            sortedWorkloadInstanceList = new List<WorkloadInstance>();
        }

        public List<WorkloadInstance> Sort()
        {
            while(nonProcessedNodes.Count > 0)
            {
                ProcessNode(nonProcessedNodes.First());
            }

            return sortedWorkloadInstanceList;
        }

        private void ProcessNode(WorkloadInstanceDFSNode node)
        {
            if (!nonProcessedNodes.Contains(node))
                return;

            if (node.inProgress)
                throw new Exception("Cyclic dependency detected between workloads.");

            node.inProgress = true;

            foreach (WorkloadInstanceDFSNode dependedNode in GetDependedNodes(node))
                ProcessNode(dependedNode);

            node.inProgress = false;
            nonProcessedNodes.Remove(node);
            sortedWorkloadInstanceList.Add(node.workloadInstance);
        }

        private IEnumerable<WorkloadInstanceDFSNode> GetDependedNodes(WorkloadInstanceDFSNode node)
        {
            List<WorkloadInstanceDFSNode> dependedNodes = new List<WorkloadInstanceDFSNode>();

            foreach (string workloadName in node.workloadInstance.VersionedWorkload.Workload.DependsOn)
            {
                dependedNodes.Concat(nameToDFSNodesMap[workloadName]);
            }

            return dependedNodes;
        }

        class WorkloadInstanceDFSNode
        {
            public WorkloadInstance workloadInstance { get; set; }
            public bool inProgress { get; set; }

            public WorkloadInstanceDFSNode(WorkloadInstance workloadInstance)
            {
                this.workloadInstance = workloadInstance;
                this.inProgress = false;
            }
        }

        private HashSet<WorkloadInstanceDFSNode> BuildNonProcessedNodes(List<WorkloadInstance> workloadInstances)
        {
            HashSet<WorkloadInstanceDFSNode> nonProcessedNodes = new HashSet<WorkloadInstanceDFSNode>();

            foreach (WorkloadInstance workloadInstance in workloadInstances)
            {
                nonProcessedNodes.Add(new WorkloadInstanceDFSNode(workloadInstance));
            }

            return nonProcessedNodes;
        }

        private Dictionary<string, List<WorkloadInstanceDFSNode>> BuildNameToDFSNodesMap(HashSet<WorkloadInstanceDFSNode> dfsNodes)
        {
            Dictionary<string, List<WorkloadInstanceDFSNode>> nameToDfsNodesMap = new Dictionary<string, List<WorkloadInstanceDFSNode>>();

            List<WorkloadInstanceDFSNode> nodesForName;
            foreach (WorkloadInstanceDFSNode node in dfsNodes)
            {
                if (!nameToDfsNodesMap.TryGetValue(node.workloadInstance.VersionedWorkload.Workload.Name, out nodesForName))
                    nodesForName = new List<WorkloadInstanceDFSNode>();

                nodesForName.Add(node);
                nameToDfsNodesMap[node.workloadInstance.VersionedWorkload.Workload.Name] = nodesForName;
            }

            return nameToDfsNodesMap;
        }
    }
}
