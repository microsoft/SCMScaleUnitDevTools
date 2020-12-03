namespace CloudAndEdgeLibs.Contracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A <see cref="Workload"/> having been assigned to a scale unit for execution.
    /// </summary>
    public sealed class WorkloadInstance : IEtag
    {
        public const string WorkloadInstances = "workloadinstances";

        /// <summary>
        /// Gets or sets the id of the <see cref="WorkloadInstance"/>.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the id of the associated LogicalEnvironment />.
        /// </summary>
        [JsonProperty("logicalEnvironmentId", NullValueHandling = NullValueHandling.Ignore)]
        public string LogicalEnvironmentId { get; set; }

        /// <summary>
        /// Gets or sets temporal physical environment assignments.
        /// </summary>
        /// <remarks>
        /// There may be only a single active environment at any one point in time.
        /// </remarks>
        [JsonProperty("executingEnvironment", NullValueHandling = NullValueHandling.Ignore)]
        public List<TemporalAssignment> ExecutingEnvironment { get; set; } = new List<TemporalAssignment>();

        /// <summary>
        /// Gets or sets the id of the associated <see cref="VersionedWorkload"/> instance.
        /// </summary>
        /// <remarks>
        /// <see cref="VersionedWorkload"/> is a container of <see cref="Workload"/> that brings additional metadata and helps supports tracking multiple versions of the
        /// same workload.
        /// </remarks>
        [JsonProperty("versionedWorkload", NullValueHandling = NullValueHandling.Ignore)]
        public VersionedWorkload VersionedWorkload { get; set; }

        /// <summary>
        /// Gets or sets the collection of <see cref="DynamicConstraintValue"/> instances. There must be an instance for each domain specified by the set of <see cref="DynamicConstraint"/>
        /// values defined on the associated <see cref="Workload"/>.
        /// </summary>
        /// <remarks>
        /// Each DynamicConstraintValue instance represents a value chosen by a logical environment admin. For example, a <see cref="DynamicConstraint"/> with a domain of "Site"
        /// may have a <see cref="DynamicConstraintValue"/> with a value of "Site1".
        /// </remarks>
        [JsonProperty("dynamicConstraintValues", NullValueHandling = NullValueHandling.Ignore)]
        public List<DynamicConstraintValue> DynamicConstraintValues { get; set; } = new List<DynamicConstraintValue>();

        /// <summary>
        /// Gets or sets whether or not an admin needs to perform an upgrade of the WorkloadInstance
        /// in order to uptake the latest Workload definition.
        /// </summary>
        /// <remarks>
        /// Scale Unit Manager scans the WorkloadInstance entity instances and checks if any are using an
        /// “old” VesionedWorkload instance. On each matching WorkloadInstance, Scale Unit Manager sets
        /// the ManuallyUpgradeTo property to the newly created VersionedWorkload’s Id property.
        ///
        /// Another background job called AdminNotifications, is periodically scanning for facts to alert
        /// upon. One such alert is any WorkloadInstance with ManuallyUpgradeTo set to a non-empty string.
        /// Upon finding this record, the AdminNotification job fetches the administrators via the defined
        /// Admins (as of this writing this should be the union of all admins defined via the Admins
        /// property on all Environment entities associated with the logical environment).
        /// </remarks>
        [JsonProperty("manuallyUpgradeTo", NullValueHandling = NullValueHandling.Ignore)]
        public string ManuallyUpgradeTo { get; set; }

        [JsonProperty("etag", NullValueHandling = NullValueHandling.Ignore)]
        public string Etag { get; set; }
    }
}
