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
    internal class AOSClient : IAOSClient
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

            string aadResource = scaleUnitInstance.AppResourceId();

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
            var path = $"{requestPathPrefix}api/services/ScaleUnitInitializationServiceGroup/ScaleUnitInitializationService/configureScaleUnit/";

            var serializedConfig = JsonConvert.SerializeObject(config);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"configuration\": {serializedConfig},\"runSync\": true}}";

            string result = await SendRequest(path, writePayload);

            ScaleUnitEnvironmentConfiguration parsed = Newtonsoft.Json.JsonConvert.DeserializeObject<ScaleUnitEnvironmentConfiguration>(result);
            return parsed;
        }

        public async Task<ScaleUnitStatus> CheckScaleUnitConfigurationStatus()
        {
            // Call requires an empty payload.
            var path = $"{requestPathPrefix}api/services/ScaleUnitInitializationServiceGroup/ScaleUnitInitializationService/checkStatus/";
            var getPayload = "{}";

            string result = await SendRequest(path, getPayload);

            ScaleUnitStatus parsed = JsonConvert.DeserializeObject<ScaleUnitStatus>(result);
            return parsed;
        }

        public async Task<List<WorkloadInstance>> WriteWorkloadInstances(List<WorkloadInstance> workloadInstances)
        {
            var path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/write/";

            // Double serialize the payload in order to avoid serialization complication issues on the AOS.
            var serializedWlInstances = JsonConvert.SerializeObject(workloadInstances);
            var serializedAsString = JsonConvert.SerializeObject(serializedWlInstances);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"workloadInstancesListAsJsonString\": {serializedAsString},\"runSync\": true}}";

            string result = await SendRequest(path, writePayload);
            string json = JsonConvert.DeserializeObject<string>(result);
            List<WorkloadInstance> parsed = JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
            return parsed;
        }

        public async Task<List<WorkloadInstance>> DeleteWorkloadInstances(List<WorkloadInstance> workloadInstances)
        {
            var path = $"{requestPathPrefix}/api/services/SysWorkloadServices/SysWorkloadInstanceService/delete/";

            // Double serialize the payload in order to avoid serialization complication issues on the AOS.
            var serializedWlInstances = JsonConvert.SerializeObject(workloadInstances);
            var serializedAsString = JsonConvert.SerializeObject(serializedWlInstances);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"workloadInstancesListAsJsonString\": {serializedAsString},\"runSync\": false}}";

            string result = await SendRequest(path, writePayload);

            string json = JsonConvert.DeserializeObject<string>(result);
            List<WorkloadInstance> parsed = JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
            return parsed;
        }

        public async Task<List<Workload>> GetWorkloads()
        {
            var path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadService/get/";

            // Call requires an empty payload.
            var getPayload = "{}";

            // The result is a double serialized string coming back from the AOS. This is done to avoid serialization "complications" on the AOS.
            string result = await SendRequest(path, getPayload);

            string json = JsonConvert.DeserializeObject<string>(result);
            List<Workload> parsed = JsonConvert.DeserializeObject<List<Workload>>(json);
            return parsed;
        }

        public async Task<List<WorkloadInstance>> GetWorkloadInstances()
        {
            var path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/get/";

            // Call requires an empty payload.
            var getPayload = "{}";

            // The result is a double serialized string coming back from the AOS. This is done to avoid serialization "complications" on the AOS.
            string result = await SendRequest(path, getPayload);

            string json = JsonConvert.DeserializeObject<string>(result);
            List<WorkloadInstance> parsed = JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
            return parsed;
        }

        public async Task<WorkloadInstanceStatus> CheckWorkloadStatus(string workloadInstanceId)
        {
            var path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/checkStatus/";

            // Call requires an empty payload.
            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\"}}";

            string result = await SendRequest(path, writePayload);

            WorkloadInstanceStatus parsed = JsonConvert.DeserializeObject<WorkloadInstanceStatus>(result);
            return parsed;
        }

        public async Task<string> GetWorkloadMovementState(string workloadInstanceId, DateTime afterDateTime)
        {
            var path = $"{requestPathPrefix}/api/services/ScaleUnitInitializationServiceGroup/ScaleUnitLifeCycleService/getWorkloadMovementState";

            // Wrap in object that allows the AOS to map in to method params on the service class.
            var writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\",\"afterDateTime\": \"{afterDateTime}\"}}";

            string result = await SendRequest(path, writePayload);

            string parsed = JsonConvert.DeserializeObject<string>(result);
            return parsed;
        }

        private async Task<String> SendRequest(string path, string payload)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(msg);
            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result;
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
