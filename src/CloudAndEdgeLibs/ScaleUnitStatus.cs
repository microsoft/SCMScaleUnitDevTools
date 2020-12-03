namespace CloudAndEdgeLibs.AOS
{
    using Newtonsoft.Json;

    public sealed class ScaleUnitStatus
    {
        [JsonProperty("health")]
        public string Health { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
