namespace CloudAndEdgeLibs.Contracts
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    /// <summary>
    /// Provides context to the Mapper underlying a Mapping.
    /// </summary>
    public sealed class MapperContext
    {
        /// <summary>
        /// Gets or sets the version of the Mapper.
        /// </summary>
        [JsonProperty("verion", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the context provided to the Mapper.
        /// </summary>
        [JsonProperty("context", NullValueHandling = NullValueHandling.Ignore)]
        public Dictionary<string, string> Context { get; set; } = new Dictionary<string, string>();
    }
}
