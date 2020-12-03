namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// DynamicConstraint defines a domain at a conceptual level via the DomainName property, and it is
    /// expected for the scale unit manager to scan all DynamicConstraint instances across all Entity
    /// and RolePath instances, generating a consolidated list. That is, if a DynamicConstraint with a
    /// DomainName of “Site” is used 30 times across 30 different Entity instances, that should be
    /// captured as a single instance and presented as such to the user. And as a consequence,
    /// DynamicConstraintValue should only have a single instance per unique DomainName. Additionally,
    /// all down-stream consumers must know how to substitute in the DynamicConstraintValue for all
    /// matching domains in the Workload “behind” the WorkloadInstance.
    /// </summary>
    public sealed class DynamicConstraintValue
    {
        /// <summary>
        /// Gets or sets the name of the target domain from within the associated workload.
        /// </summary>
        [JsonProperty("domainName", NullValueHandling = NullValueHandling.Ignore)]
        public string DomainName { get; set; }

        /// <summary>
        /// Gets or sets the domain value.
        /// </summary>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }
    }
}
