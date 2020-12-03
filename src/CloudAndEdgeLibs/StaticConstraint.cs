namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    /// <summary>
    /// Represents a constraint to be applied to all instances of the containing <see cref="Workload"/>, with no input required from a logical environment administrator.
    /// </summary>
    public sealed class StaticConstraint
    {
        /// <summary>
        /// Gets or sets the constraint's description.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the field to be targeted by the constraint.
        /// </summary>
        [JsonProperty("field", NullValueHandling = NullValueHandling.Ignore)]
        public string Field { get; set; }

        /// <summary>
        /// Gets or sets the constraint's value as a string.
        /// </summary>
        /// <remarks>
        /// Type conversion is handled automatically by referring to the target field's type.
        /// </remarks>
        [JsonProperty("value", NullValueHandling = NullValueHandling.Ignore)]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the operator to use when applying the constraint.
        /// </summary>
        [JsonProperty("operator", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(StringEnumConverter))]
        public ConstraintOperator Operator { get; set; }
    }
}
