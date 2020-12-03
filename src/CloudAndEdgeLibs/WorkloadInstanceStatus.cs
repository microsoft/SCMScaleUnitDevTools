namespace CloudAndEdgeLibs.AOS
{
    using Newtonsoft.Json;

    public sealed class WorkloadInstanceStatus
    {
        [JsonProperty("health")]
        public string Health { get; set; }

        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
