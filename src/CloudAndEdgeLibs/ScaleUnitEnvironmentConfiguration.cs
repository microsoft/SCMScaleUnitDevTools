namespace CloudAndEdgeLibs.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Newtonsoft.Json;

    /// <summary>
    /// Representation of the ScaleUnitEnvironmentConfiguration class defined in X++.
    /// </summary>
    public sealed class ScaleUnitEnvironmentConfiguration
    {
        [JsonProperty("hubS2SEncryptedSecret")]
        public string HubS2SEncryptedSecret { get; set; }

        [JsonProperty("hubResourceId")]
        public string HubResourceId { get; set; }

        [JsonProperty("appTenant")]
        public string AppTenant { get; set; }

        [JsonProperty("scaleUnitType")]
        public string ScaleUnitType { get; set; }

        [JsonProperty("hubUrl")]
        public string HubUrl { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }
    }
}
