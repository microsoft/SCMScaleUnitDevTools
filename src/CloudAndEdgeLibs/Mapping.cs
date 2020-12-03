namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Used to express a join through a hierarchy of tables, using table roles.
    /// </summary>
    public sealed class Mapping
    {
        /// <summary>
        /// Gets or sets the name of the workload.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the table containing the <see cref="SourceFieldName"/> which requires a mapping.
        /// </summary>
        [JsonProperty("sourceTableName", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceTableName { get; set; }

        /// <summary>
        /// Gets or sets the source field which requires processing before being applied to a target.
        /// </summary>
        [JsonProperty("sourceFieldName", NullValueHandling = NullValueHandling.Ignore)]
        public string SourceFieldName { get; set; }

        /// <summary>
        /// Gets or sets the type of Mapper supported by the Data Feeds Service.
        /// </summary>
        /// <remarks>
        /// The Mapper is referenced by a basic identifying Name, and is not fully namespaced.
        /// </remarks>
        [JsonProperty("mapperType", NullValueHandling = NullValueHandling.Ignore)]
        public string MapperType { get; set; }

        /// <summary>
        /// Gets or sets the context to be provided to the Mapper when applying the mapping.
        /// </summary>
        [JsonProperty("mapperContext", NullValueHandling = NullValueHandling.Ignore)]
        public MapperContext MapperContext { get; set; }
    }
}
