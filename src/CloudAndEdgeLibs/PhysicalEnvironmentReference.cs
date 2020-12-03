namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;

    public class PhysicalEnvironmentReference
    {
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id { get; set; }

        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        [JsonProperty("aosUrl", NullValueHandling = NullValueHandling.Ignore)]
        public string AOSUrl { get; set; }

        [JsonProperty("scaleUnitId", NullValueHandling = NullValueHandling.Ignore)]
        public string ScaleUnitId { get; set; }
    }
}
