namespace CloudAndEdgeLibs.Contracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// A collection of tables together representing a piece of data that must be thought of as a single logical unit, including its set of
    /// access permissions.
    /// </summary>
    /// <remarks>
    /// It is possible for a table to span multiple entities. In that case, a union of access permissions will be taken.
    /// </remarks>
    public sealed class Entity
    {
        /// <summary>
        /// Gets or sets the name of this entity.
        /// </summary>
        /// <remarks>
        /// This name must be unique within the containing workload.
        /// </remarks>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table representing the root of this entity.
        /// </summary>
        [JsonProperty("rootTableName", NullValueHandling = NullValueHandling.Ignore)]
        public string RootTableName { get; set; }

        /// <summary>
        /// Gets or sets the role paths to be included with this entity via joins.
        /// </summary>
        [JsonProperty("includedRolePaths", NullValueHandling = NullValueHandling.Ignore)]
        public List<RolePath> IncludedRolePaths { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not read access is required.
        /// </summary>
        [JsonProperty("reads", NullValueHandling = NullValueHandling.Ignore)]
        public bool Reads { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not update access is required.
        /// </summary>
        [JsonProperty("updates", NullValueHandling = NullValueHandling.Ignore)]
        public bool Updates { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not delete access is required.
        /// </summary>
        [JsonProperty("deletes", NullValueHandling = NullValueHandling.Ignore)]
        public bool Deletes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not insert access is required.
        /// </summary>
        [JsonProperty("inserts", NullValueHandling = NullValueHandling.Ignore)]
        public bool Inserts { get; set; }

        /// <summary>
        /// Gets or sets the constraints that apply to all instances of the containing workload.
        /// </summary>
        [JsonProperty("staticConstraints", NullValueHandling = NullValueHandling.Ignore)]
        public List<StaticConstraint> StaticConstraints { get; set; }

        /// <summary>
        /// Gets or sets the constraints to be specified by an administrator when a instance of this workload is created.
        /// </summary>
        [JsonProperty("dynamicConstraints", NullValueHandling = NullValueHandling.Ignore)]
        public List<DynamicConstraint> DynamicConstraints { get; set; }

        /// <summary>
        /// Gets or sets the set of fields to be included in the select list.
        /// </summary>
        /// <remarks>
        /// If your goal is to exclude a set of fields, then consider instead configuring the <see cref="ProjectionExclusionList"/> property. <see cref="ProjectionList"/>, if specified, will need to
        /// be maintained as fields are added to the underlying table.
        /// </remarks>
        [JsonProperty("projectionList", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ProjectionList { get; set; }

        /// <summary>
        /// Gets or sets the set of fields to be excluded from the select list.
        /// </summary>
        /// <remarks>
        /// When <see cref="ProjectionList"/> is not provided the behavior is to select all fields save for those included here. If <see cref="ProjectionList"/> is provided
        /// this setting is ignored.
        /// </remarks>
        [JsonProperty("projectionExclusionList", NullValueHandling = NullValueHandling.Ignore)]
        public List<string> ProjectionExclusionList { get; set; }
    }
}
