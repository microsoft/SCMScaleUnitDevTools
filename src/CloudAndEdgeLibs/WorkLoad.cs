namespace CloudAndEdgeLibs.Contracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A Workload is a collection of tables, permissions, and associated code together representing a business process that can be executed in isolation from other
    /// business processes. Manufacturing Execution and Warehouse Execution are examples of Workloads defined by Microsoft.
    /// </summary>
    public sealed class Workload
    {
        public enum SyncDirection
        {
            HubToSpoke = 0,
            SpokeToHub = 1,
            BothWays = 2,
        }

        /// <summary>
        /// Gets or sets the name of the workload.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the entities required by the workload but that are populated by the hub and copied
        /// to wherever the workload is running.
        /// </summary>
        [JsonProperty("hubEntities", NullValueHandling = NullValueHandling.Ignore)]
        public List<Entity> HubEntities { get; set; } = new List<Entity>();

        /// <summary>
        /// Gets or sets the entities required by the workload.
        /// </summary>
        [JsonProperty("entities", NullValueHandling = NullValueHandling.Ignore)]
        public List<Entity> Entities { get; set; } = new List<Entity>();

        /// <summary>
        /// Gets or sets the AX privileges required by the workload running on the target environment.
        /// </summary>
        [JsonProperty("requiredPrivileges", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RequiredPrivileges { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the AX features required by the workload running on the target environment.
        /// </summary>
        [JsonProperty("requiredFeatures", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> RequiredFeatures { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the mappings used to maintain compatibility between varied environments executing a workload.
        /// </summary>
        [JsonProperty("mappings", NullValueHandling = NullValueHandling.Ignore)]
        public List<Mapping> Mappings { get; set; } = new List<Mapping>();

        /// <summary>
        /// Gets or sets the metrics to be used by environment admins to track the health and performance of a workload
        /// across a logical environment.
        /// </summary>
        [JsonProperty("metrics", NullValueHandling = NullValueHandling.Ignore)]
        public List<Metric> Metrics { get; set; } = new List<Metric>();

        /// <summary>
        /// Gets or sets list of workload names that this workload depends on. Dependent workloads need to be installed
        /// prior to installation of this workload.
        /// </summary>
        [JsonProperty("dependsOn", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DependsOn { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets synchronization direction of this workload.
        /// </summary>
        [JsonProperty("synchronizationDirection", NullValueHandling = NullValueHandling.Ignore)]
        public SyncDirection SynchronizationDirection { get; set; }

        /// <summary>
        /// Gets or sets system metadata required by this workload. Reserved for system use.
        /// </summary>
        /// <remarks>
        /// The key of the dictionary represents the type of dependency, while the value is a string expressing an instance of the type.
        /// </remarks>
        [JsonProperty("__dependentSystemMetadata", NullValueHandling = NullValueHandling.Ignore)]
#pragma warning disable SA1300 // Element should begin with upper-case letter
        public SortedSet<SystemMetadata> __DependentSystemMetadata { get; set; } = new SortedSet<SystemMetadata>();
#pragma warning restore SA1300 // Element should begin with upper-case letter
    }
}
