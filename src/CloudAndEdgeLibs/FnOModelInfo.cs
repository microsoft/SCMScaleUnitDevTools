namespace CloudAndEdgeLibs
{
    using System;
    using Newtonsoft.Json;

    public sealed class FnOModelInfo : IComparable<FnOModelInfo>
    {
        /// <summary>
        /// Gets or sets the name of the Finance and Operations model.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the major component of the version.
        /// </summary>
        [JsonProperty("versionMajorComponent", NullValueHandling = NullValueHandling.Ignore)]
        public string VersionMajorComponent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the minor component of the version.
        /// </summary>
        [JsonProperty("versionMinorComponent", NullValueHandling = NullValueHandling.Ignore)]
        public string VersionMinorComponent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the build component of the version.
        /// </summary>
        [JsonProperty("versionBuildComponent", NullValueHandling = NullValueHandling.Ignore)]
        public string VersionBuildComponent { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the revision component of the version.
        /// </summary>
        [JsonProperty("versionRevisionComponent", NullValueHandling = NullValueHandling.Ignore)]
        public string VersionRevisionComponent { get; set; } = string.Empty;

        public string AggregatedVersion
        {
            get
            {
                return $"{VersionMajorComponent}.{VersionMinorComponent}.{VersionRevisionComponent}.{VersionBuildComponent}";
            }
        }

        public int CompareTo(FnOModelInfo other)
        {
            return this.Name.CompareTo(other?.Name);
        }
    }
}
