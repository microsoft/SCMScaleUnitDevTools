namespace CloudAndEdgeLibs.Contracts
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Container of <see cref="Workload"/> used to track related metadata, such as app and plat versions.
    /// </summary>
    public sealed class VersionedWorkload : IEtag
    {
        public const string VersionedWorkloads = "versionedworkloads";

        /// <summary>
        /// Gets or sets the package id of the platform that helped generated the workload definition.
        /// </summary>
        /// <remarks>
        /// This is captured from sources outside of the AOS at the time the new/unique workload
        /// was first identified.
        /// </remarks>
        [JsonProperty("platformPackageId", NullValueHandling = NullValueHandling.Ignore)]
        public string PlatformPackageId { get; set; }

        /// <summary>
        /// Gets or sets the package id of the application that helped generated the workload definition.
        /// </summary>
        /// <remarks>
        /// This is captured from sources outside of the AOS at the time the new/unique workload
        /// was first identified.
        /// </remarks>
        [JsonProperty("applicationPackageId", NullValueHandling = NullValueHandling.Ignore)]
        public string ApplicationPackageId { get; set; }

        /// <summary>
        /// Gets or sets the package/asset id of the customization that helped generated the workload definition.
        /// </summary>
        /// <remarks>
        /// This is captured from sources outside of the AOS at the time the new/unique workload
        /// was first identified.
        /// </remarks>
        [JsonProperty("customizationPackageId", NullValueHandling = NullValueHandling.Ignore)]
        public string CustomizationPackageId { get; set; }

        /// <summary>
        /// Gets or sets metadata from the environment that generated the workload definition.
        /// </summary>
        /// <remarks>
        /// In normal flows, the hub environment is the source of the workload definition.
        /// </remarks>
        [JsonProperty("environmentMetadata", NullValueHandling = NullValueHandling.Ignore)]
        public FnOEnvironmentDeploymentMetadata EnvironmentMetadata { get; set; }

        /// <summary>
        /// Gets or sets the hash of the workload's stringified json representation.
        /// </summary>
        /// <remarks>
        /// Used to track uniqueness of a workload definition.
        /// </remarks>
        [JsonProperty("hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the name of the hash provider used to produce the hash.
        /// </summary>
        [JsonProperty("hashProvider", NullValueHandling = NullValueHandling.Ignore)]
        public string HashProvider { get; set; }

        /// <summary>
        /// Gets or sets the id of the LogicalEnvironment used to generate the workload definition.
        /// </summary>
        [JsonProperty("logicalEnvironmentId", NullValueHandling = NullValueHandling.Ignore)]
        public string LogicalEnvironmentId { get; set; }

        /// <summary>
        /// Gets or sets the surrogate key of the versioned workload.
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the actual workload definition.
        /// </summary>
        [JsonProperty("workload", NullValueHandling = NullValueHandling.Ignore)]
        public Workload Workload { get; set; }

        /// <summary>
        /// Gets or sets date time when the workload was discovered.
        /// </summary>
        [JsonProperty("discoveredDateTime", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime DiscoveredDateTime { get; set; }

        /// <summary>
        /// Gets or sets list of versioned workload ids that this workload depends on.
        /// </summary>
        [JsonProperty("dependsOn", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> DependsOn { get; set; }

        /// <summary>
        /// Gets or sets list of dynamic constraints.
        /// </summary>
        [JsonProperty("dynamicConstraints", NullValueHandling = NullValueHandling.Ignore)]
        public List<DynamicConstraint> DynamicConstraints { get; set; }

        [JsonProperty("etag", NullValueHandling = NullValueHandling.Ignore)]
        public string Etag { get; set; }
    }
}
