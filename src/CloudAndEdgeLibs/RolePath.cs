namespace CloudAndEdgeLibs.Contracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Used to express a join through a hierarchy of tables, using table roles.
    /// </summary>
    public sealed class RolePath
    {
        /// <summary>
        /// Gets or sets the name of the workload.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the path of roles, using + and - as delimimters. + indicates a left outer join, while - indicates
        /// and inner join.
        /// </summary>
        [JsonProperty("path", NullValueHandling = NullValueHandling.Ignore)]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the projection list for the leaf table.
        /// </summary>
        [JsonProperty("projection", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> Projection { get; set; } = new List<string>();

        /// <summary>
        /// Gets or sets the static constraints to apply to the leaf table.
        /// </summary>
        [JsonProperty("staticConstraints", NullValueHandling = NullValueHandling.Ignore)]
        public List<StaticConstraint> StaticConstraints { get; set; } = new List<StaticConstraint>();
    }
}
