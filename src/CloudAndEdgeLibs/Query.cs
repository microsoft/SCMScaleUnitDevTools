namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;

    public sealed class Query
    {
        /// <summary>
        /// Gets or sets the serialized query.
        /// </summary>
        [JsonProperty("serialized", NullValueHandling = NullValueHandling.Ignore)]
        public string Serialized { get; set; }

        /// <summary>
        /// Gets or sets the serialization format of the query.
        /// </summary>
        [JsonProperty("serializationFormat", NullValueHandling = NullValueHandling.Ignore)]
        public string SerializationFormat { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the query is to be applied when performing full sync and trickle feed.
        /// </summary>
        [JsonProperty("isForFullSyncAndTrickleFeed", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsForFullSyncAndTrickleFeed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the query is to be applied when performing scale unit target marking.
        /// </summary>
        [JsonProperty("isForMarking", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsForMarking { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the query is to be applied when performing scale unit ownership changes.
        /// </summary>
        [JsonProperty("isForOwnershipChanging", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsForOwnershipChanging { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the query is to be applied when performing reverse seeding.
        /// </summary>
        [JsonProperty("isForReverseSeeding", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsForReverseSeeding { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the query is to be applied when performing scale unit cleanup.
        /// </summary>
        [JsonProperty("isForDelete", NullValueHandling = NullValueHandling.Ignore)]
        public bool IsForDelete { get; set; }
    }
}
