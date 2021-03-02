namespace CloudAndEdgeLibs
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public sealed class FnOEnvironmentDeploymentMetadata
    {
        [JsonProperty("models", NullValueHandling = NullValueHandling.Ignore)]
        public SortedSet<FnOModelInfo> Models { get; set; } = new SortedSet<FnOModelInfo>();
    }
}
