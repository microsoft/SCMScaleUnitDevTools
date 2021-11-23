using System;
using System.Collections.Generic;
using System.Net;
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
            httpClient.Timeout = TimeSpan.FromMinutes(20);
            httpClient.DefaultRequestHeaders.Add("Authorization", await OAuthHelper.GetAuthenticationHeader(aadTenant, aadClientAppId, aadClientAppSecret, aadResource));

            var aosClient = new AOSClient(httpClient, scaleUnitInstance.AOSRequestPathPrefix());

            await aosClient.Ping();

            return aosClient;
        }

        private async Task Ping()
        {
            string path = $"{requestPathPrefix}ping/";
            string getPayload = "{}";

            _ = await SendRequest(path, getPayload);
        }

        public async Task<ScaleUnitEnvironmentConfiguration> WriteScaleUnitConfiguration(ScaleUnitEnvironmentConfiguration config)
        {
            string path = $"{requestPathPrefix}api/services/ScaleUnitInitializationServiceGroup/ScaleUnitInitializationService/configureScaleUnit/";

            string serializedConfig = JsonConvert.SerializeObject(config);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            string writePayload = $"{{\"configuration\": {serializedConfig},\"runSync\": true}}";

            string result = await SendRequest(path, writePayload);

            ScaleUnitEnvironmentConfiguration parsed = JsonConvert.DeserializeObject<ScaleUnitEnvironmentConfiguration>(result);
            return parsed;
        }

        public async Task<ScaleUnitStatus> CheckScaleUnitConfigurationStatus()
        {
            // Call requires an empty payload.
            string path = $"{requestPathPrefix}api/services/ScaleUnitInitializationServiceGroup/ScaleUnitInitializationService/checkStatus/";
            string getPayload = "{}";

            string result = await SendRequest(path, getPayload);

            ScaleUnitStatus parsed = JsonConvert.DeserializeObject<ScaleUnitStatus>(result);
            return parsed;
        }

        public async Task<List<WorkloadInstance>> WriteWorkloadInstances(List<WorkloadInstance> workloadInstances)
        {
            string path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/write/";

            // Double serialize the payload in order to avoid serialization complication issues on the AOS.
            string serializedWlInstances = JsonConvert.SerializeObject(workloadInstances);
            string serializedAsString = JsonConvert.SerializeObject(serializedWlInstances);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            string writePayload = $"{{\"workloadInstancesListAsJsonString\": {serializedAsString},\"runSync\": true}}";

            string result = await SendRequest(path, writePayload);
            string json = JsonConvert.DeserializeObject<string>(result);
            List<WorkloadInstance> parsed = JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
            return parsed;
        }

        public async Task<List<WorkloadInstance>> DeleteWorkloadInstances(List<WorkloadInstance> workloadInstances)
        {
            string path = $"{requestPathPrefix}/api/services/SysWorkloadServices/SysWorkloadInstanceService/delete/";

            // Double serialize the payload in order to avoid serialization complication issues on the AOS.
            string serializedWlInstances = JsonConvert.SerializeObject(workloadInstances);
            string serializedAsString = JsonConvert.SerializeObject(serializedWlInstances);

            // Wrap in object that allows the AOS to map in to method params on the service class.
            string writePayload = $"{{\"workloadInstancesListAsJsonString\": {serializedAsString},\"runSync\": false}}";

            string result = await SendRequest(path, writePayload);

            string json = JsonConvert.DeserializeObject<string>(result);
            List<WorkloadInstance> parsed = JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
            return parsed;
        }

        public async Task<List<Workload>> GetWorkloads()
        {
            string path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadService/get/";

            // Call requires an empty payload.
            string getPayload = "{}";

            // The result is a double serialized string coming back from the AOS. This is done to avoid serialization "complications" on the AOS.
            string result = await SendRequest(path, getPayload);

            string json = JsonConvert.DeserializeObject<string>(result);
            List<Workload> parsed = JsonConvert.DeserializeObject<List<Workload>>(json);
            return parsed;
        }

        public async Task<List<WorkloadInstance>> GetWorkloadInstances()
        {
            string path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/get/";

            // Call requires an empty payload.
            string getPayload = "{}";

            // The result is a double serialized string coming back from the AOS. This is done to avoid serialization "complications" on the AOS.
            string result = await SendRequest(path, getPayload);

            string json = JsonConvert.DeserializeObject<string>(result);
            List<WorkloadInstance> parsed = JsonConvert.DeserializeObject<List<WorkloadInstance>>(json);
            return parsed;
        }

        public async Task<WorkloadInstanceStatus> CheckWorkloadStatus(string workloadInstanceId)
        {
            string path = $"{requestPathPrefix}api/services/SysWorkloadServices/SysWorkloadInstanceService/checkStatus/";

            // Wrap in object that allows the AOS to map in to method params on the service class.
            string writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\"}}";

            string result = await SendRequest(path, writePayload);

            WorkloadInstanceStatus parsed = JsonConvert.DeserializeObject<WorkloadInstanceStatus>(result);
            return parsed;
        }

        public async Task<string> GetWorkloadMovementState(string workloadInstanceId, DateTime afterDateTime)
        {
            string path = $"{requestPathPrefix}/api/services/ScaleUnitInitializationServiceGroup/ScaleUnitLifeCycleService/getWorkloadMovementState";

            // Wrap in object that allows the AOS to map in to method params on the service class.
            string writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\",\"afterDateTime\": \"{afterDateTime}\"}}";

            string result = await SendRequest(path, writePayload);

            string parsed = JsonConvert.DeserializeObject<string>(result);
            return parsed;
        }

        public async Task DrainWorkload(string workloadInstanceId)
        {
            string path = $"{requestPathPrefix}/api/services/SysWorkloadServices/SysWorkloadInstanceService/drain/";

            string writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\"}}";

            await SendRequest(path, writePayload);
        }

        public async Task StartWorkload(string workloadInstanceId)
        {
            string path = $"{requestPathPrefix}/api/services/SysWorkloadServices/SysWorkloadInstanceService/start/";

            string writePayload = $"{{\"workloadInstanceId\": \"{workloadInstanceId}\"}}";

            await SendRequest(path, writePayload);
        }

        private async Task<string> SendRequest(string path, string payload)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(payload, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response;
            try
            {
                response = await httpClient.SendAsync(msg);
            }
            catch (TaskCanceledException)
            {
                throw new TaskCanceledException("The server did not respond before timeout");
            }

            string result = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                return result;
            }
            else
            {
                throw RequestFailure(response.StatusCode, result);
            }
        }

        private static Exception RequestFailure(HttpStatusCode statusCode, string response)
        {
            return new AOSClientError(statusCode, response);
        }
    }
}
