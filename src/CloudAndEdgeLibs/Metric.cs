namespace CloudAndEdgeLibs.Contracts
{
    using Newtonsoft.Json;

    /// <summary>
    /// Defines a metric meant to drive insights into a workload.
    /// </summary>
    /// <remarks>
    /// Metrics are meant to drive insights into workloads and can be viewed by adminstrators at https://sum.dynamics.com. The metrics displayed should allow an
    /// administrator to configure an optimized logical environment topology, providing maximum up time and performance. Metrics to consider are: latency in milliseconds
    /// from localized clients, process performance in seconds per critical task, et. al.
    /// </remarks>
    public sealed class Metric
    {
        /// <summary>
        /// Gets or sets the name of the workload.
        /// </summary>
        [JsonProperty("name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the location from which to retrieve measurements of this metric.
        /// </summary>
        [JsonProperty("location", NullValueHandling = NullValueHandling.Ignore)]
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets the unit of measure for this metric.
        /// </summary>
        [JsonProperty("units", NullValueHandling = NullValueHandling.Ignore)]
        public string Units { get; set; }

        /// <summary>
        /// Gets or sets the user displayable description of this metric.
        /// </summary>
        [JsonProperty("description", NullValueHandling = NullValueHandling.Ignore)]
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the threshold at which this metric considers the system to be experiencing a low severity issue.
        /// </summary>
        [JsonProperty("lowSeverityThreshold", NullValueHandling = NullValueHandling.Ignore)]
        public double LowSeverityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the threshold at which this metric considers the system to be experiencing a medium severity issue.
        /// </summary>
        [JsonProperty("mediumSeverityThreshold", NullValueHandling = NullValueHandling.Ignore)]
        public double MediumSeverityThreshold { get; set; }

        /// <summary>
        /// Gets or sets the threshold at which this metric considers the system to be experiencing a medium severity issue.
        /// </summary>
        [JsonProperty("highSeverityThreshold", NullValueHandling = NullValueHandling.Ignore)]
        public double HighSeverityThreshold { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not severity should be inversly proportional to the value.
        /// </summary>
        /// <remarks>
        /// This flips the comparison of the threshold values. In other words, a lower value is worse than a high value.
        /// </remarks>
        [JsonProperty("severityInverseProportionalityMode", NullValueHandling = NullValueHandling.Ignore)]
        public bool SeverityInverseProportionalityMode { get; set; }
    }
}
