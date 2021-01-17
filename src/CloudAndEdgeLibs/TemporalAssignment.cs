namespace CloudAndEdgeLibs.Contracts
{
    using System;
    using Newtonsoft.Json;

    /// <summary>
    /// Represents a temporal assignment of a workload instance to a physical environment.
    /// </summary>
    public sealed class TemporalAssignment
    {
        public const string WorkloadInstanceOwnerships = "workloadinstanceownerships";

        /// <summary>
        /// Gets or sets the date and time at which the assignment is effective.
        /// </summary>
        [JsonProperty("effectiveDate", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets the physical environment on which to run the associated <see cref="WorkloadInstance"/>.
        /// </summary>
        [JsonProperty("environment", NullValueHandling = NullValueHandling.Ignore)]
        public PhysicalEnvironmentReference Environment { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not historical data should be in a read only state.
        /// </summary>
        /// <remarks>
        /// This is only applicable to expired assignments.
        /// </remarks>
        [JsonProperty("disableDataMaintenance", NullValueHandling = NullValueHandling.Ignore)]
        public bool DisableDataMaintenance { get; set; }
    }
}
