using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;
using Newtonsoft.Json;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    class AOSClient
    {
        private readonly HttpClient httpClient;
        private readonly string requestPathPrefix = "";

        private AOSClient(HttpClient httpClient, string requestPathPrefix = "")
        {
            this.httpClient = httpClient;
            this.requestPathPrefix = requestPathPrefix;
        }

        public static async Task<AOSClient> Construct(ScaleUnitInstance scaleUnitInstance)
        {
            string aadTenant = scaleUnitInstance.AuthConfiguration.Authority;
            string aadClientAppId = scaleUnitInstance.AuthConfiguration.AppId;
            string aadClientAppSecret = scaleUnitInstance.AuthConfiguration.AppSecret;

            string aadResource = scaleUnitInstance.ResourceId();

            var httpClient = new HttpClient
            {
                BaseAddress = new Uri(scaleUnitInstance.Endpoint())
            };
            httpClient.DefaultRequestHeaders.Add("Authorization", await OAuthHelper.GetAuthenticationHeader(aadTenant, aadClientAppId, aadClientAppSecret, aadResource));

            AOSClient client = new AOSClient(httpClient, scaleUnitInstance.AOSRequestPathPrefix());

            return client;
        }

        public async Task<ScaleUnitEnvironmentConfiguration> WriteScaleUnitConfiguration(ScaleUnitEnvironmentConfiguration config)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, $"{requestPathPrefix}api/services/ScaleUnitInitializationServiceGroup/ScaleUnitInitializationService/configureScaleUnit/");

            var serializedConfig = Newtonsoft.Json.JsonConvert.SerializeObject(config);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"configuration\": {serializedConfig},\"runSync\": true}}";
            msg.Content = new StringContent(writePayload, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(msg);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ScaleUnitEnvironmentConfiguration parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<ScaleUnitEnvironmentConfiguration>(result);
                return parsed;
            }
            else
            {
                throw RequestFailure((int)response.StatusCode, result);
            }
        }

        public async Task<ScaleUnitStatus> CheckScaleUnitConfigurationStatus()
        {
            // Call requires an empty payload.
            var msg = new HttpRequestMessage(HttpMethod.Post, $"{requestPathPrefix}api/services/ScaleUnitInitializationServiceGroup/ScaleUnitInitializationService/checkStatus/");
            var getPayload = "{}";
            msg.Content = new StringContent(getPayload, Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(msg);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                ScaleUnitStatus parsed = JsonConvert.DeserializeObject<ScaleUnitStatus>(result);
                return parsed;
            }
            else
            {
                throw RequestFailure((int)response.StatusCode, result);
            }
        }

        public async Task<List<WorkloadInstance>> WriteWorkloadInstances(List<WorkloadInstance> workloadInstances)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/write/");

            // Double serialize the payload in order to avoid serialization complication issues on the AOS.
            var serializedWlInstances = Newtonsoft.Json.JsonConvert.SerializeObject(workloadInstances);
            var serializedAsString = Newtonsoft.Json.JsonConvert.SerializeObject(serializedWlInstances);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"workloadInstancesListAsJsonString\": {serializedAsString},\"runSync\": true}}";
            msg.Content = new StringContent(writePayload, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(msg);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                string json = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(result);
                List<WorkloadInstance> parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
                return parsed;
            }
            else
            {
                throw RequestFailure((int)response.StatusCode, result);
            }
        }

        public async Task<List<Workload>> GetWorkloads()
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadService/get/");

            // Call requires an empty payload.
            var getPayload = "{}";
            msg.Content = new StringContent(getPayload, Encoding.UTF8, "application/json");
            var response = await httpClient.SendAsync(msg);

            // The result is a double serialized string coming back from the AOS. This is done to avoid serialization "complications" on the AOS.
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                string json = Newtonsoft.Json.JsonConvert.DeserializeObject<string>(result);
                List<Workload> parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Workload>>(json);
                return parsed;
            }
            else
            {
                throw RequestFailure((int)response.StatusCode, result);
            }
        }

        public async Task<WorkloadInstanceStatus> CheckWorkloadStatus(string workloadInstanceId)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/checkStatus/");

            // Call requires an empty payload.
            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\"}}";
            msg.Content = new StringContent(writePayload, Encoding.UTF8, "application/json");

            var response = await httpClient.SendAsync(msg);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                WorkloadInstanceStatus parsed = JsonConvert.DeserializeObject<WorkloadInstanceStatus>(result);
                return parsed;
            }
            else
            {
                throw RequestFailure((int)response.StatusCode, result);
            }
        }

        private static Exception RequestFailure(int statusCode, string response)
        {
            return new Exception($"[{statusCode}]: Error - {response}");
        }
    }
}
