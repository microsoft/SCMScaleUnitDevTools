namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// DynamicConstraint defines a domain such as "Site" at a conceptual level via the DomainName
    /// property. It also associates the domain with a field, with the idea that domains can be
    /// associated to any number of fields and later consolidated by domain name.
    /// </summary>
    /// <remarks>
    /// It is expected for the scale unit manager to scan all DynamicConstraint instances
    /// across all Entity and RolePath instances, generating a consolidated list. That is, if a
    /// DynamicConstraint with a DomainName of “Site” is used 30 times across 30 different Entity instances,
    /// that should be captured as a single instance and presented as such to the user. And as a
    /// consequence, DynamicConstraintValue should only have a single instance per unique DomainName.
    /// Additionally, all down-stream consumers must know how to substitute in the DynamicConstraintValue
    /// for all matching domains in the Workload “behind” the WorkloadInstance.
    /// </remarks>
    public sealed class DynamicConstraint
    {
        /// <summary>
        /// Gets or sets the name of the domain, such as "Site". This domain should be re-used across multiple <see cref="DynamicConstraint"/> instances where the same value
        /// would be entered for the domain for a single instance of the containing workload.
        /// </summary>
        [JsonProperty("domainName", NullValueHandling = NullValueHandling.Ignore)]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the description of the constraint.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field on which to apply the constraint.
        /// </summary>
        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the operator to apply when applying the constraint.
        /// </summary>
        [JsonProperty("operator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConstraintOperator Operator { get; set; }

        /// <summary>
        /// Gets or sets the endpoint to call for fetching the set of values supported by the domain.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }
    }
}
