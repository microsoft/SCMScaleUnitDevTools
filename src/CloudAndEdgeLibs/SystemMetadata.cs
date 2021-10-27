namespace CloudAndEdgeLibs.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;

    /// <summary>
    /// Expresses system metadata used internally by the workload. This should only be consumed internally by Microsoft processes.
    /// </summary>
    public sealed class SystemMetadata : IComparable<SystemMetadata>
    {
        /// <summary>
        /// Gets or sets the type of system metadata.
        /// </summary>
        [JsonProperty("metadataTypeName", NullValueHandling = NullValueHandling.Ignore)]
        public string MetadataTypeName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier, which should be unique within the metadata type.
        /// </summary>
        /// <remarks>
        /// Note that uniqueness is not enforced.
        /// </remarks>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets properties for this metadata item.
        /// </summary>
        [JsonProperty("properties", NullValueHandling = NullValueHandling.Ignore)]
        public SortedDictionary<string, string> Properties { get; set; } = new SortedDictionary<string, string>();

        public static SystemMetadata Find(IEnumerable<SystemMetadata> list, string type, string id)
        {
            return list.FirstOrDefault((metadata) => metadata.Id.Equals(id, StringComparison.OrdinalIgnoreCase) && metadata.MetadataTypeName.Equals(type, StringComparison.OrdinalIgnoreCase));
        }

        public int CompareTo(SystemMetadata other)
        {
            int typeComparison = this.MetadataTypeName.CompareTo(other.MetadataTypeName);
            if (typeComparison == 0)
            {
                return this.Id.CompareTo(other.Id);
            }

            return typeComparison;
        }
    }
}
