using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace ScaleUnitManagement.Utilities
{
    public class Config
    {
        public const int RetryCount = 5;
        public static string ConfigEncryptorExePath = $@"{ServiceVolume()}\AOSService\webroot\bin\Microsoft.Dynamics.AX.Framework.ConfigEncryptor.exe";
        public static string LogicalEnvironmentId = "82099c35-019e-45cf-9233-630ca5dd69ec";
        public static string WorkloadDefinitionHash = "c2f309922ef4fc1f1a418ae31ebf85763f8a836e355062e6489ff12e3c438c0d";
        public static string ScaleUnitEnvironmentId = "a4557a4e-6980-4e1b-b7c7-661793b4a098";
        public static string DynamicsBatchExePath = $@"{ServiceVolume()}\AOSService\webroot\bin\Batch.exe";

        // Hub Configs
        public static string HubSiteRoot = $@"{ServiceVolume()}\AOSService\webroot";
        public static string HubWebConfigPath = $@"{HubSiteRoot}\web.config";
        public static string HubBatchName = "DynamicsAxBatch";
        public static string HubAppPoolName = "AOSService";

        // ScaleUnit Configs
        public static string ScaleUnitSiteRoot = $@"{ServiceVolume()}\AOSService\webrootspoke";
        public static string ScaleUnitWebConfigPath = $@"{ScaleUnitSiteRoot}\web.config";
        public static string ScaleUnitWifServicesConfigPath = $@"{ScaleUnitSiteRoot}\wif.services.config";
        public static string ScaleUnitBatchName = "DynamicsAXScaleUnitBatch";
        public static string ScaleUnitAppPoolName = "AOSServiceScaleUnit";

        private static CloudAndEdgeConfiguration UserConfig { get; set; }

        private static CloudAndEdgeConfiguration UserConfiguration()
        {
            if (UserConfig == null)
            {
                UserConfig = LoadUserConfig.LoadConfig();
                Validate();
            }

            return UserConfig;
        }

        private static string SanitizeDomain(string domainName)
        {
            // trim surrounding whitespace
            domainName = domainName.Trim();

            // remove https?://
            domainName = Regex.Replace(domainName, @"^(http(s)?://)?", string.Empty);

            // * ensure no "/" suffix
            domainName = domainName.TrimEnd('/');

            return domainName;
        }

        public static string AxDbName() { return UserConfiguration().AxDbName; }
        public static string AxScaleUnitDbName() { return UserConfiguration().AxScaleUnitDbName; }
        public static string AppId() { return UserConfiguration().AADConfiguration.AppId; }
        public static string AppSecret() { return UserConfiguration().AADConfiguration.AppSecret; }
        public static string Authority() { return UserConfiguration().AADConfiguration.Authority; }
        public static string InterAOSAppId() { return UserConfiguration().ScaleUnitConfiguration.InterAOSAppId; }
        public static string InterAOSAppSecret() { return UserConfiguration().ScaleUnitConfiguration.InterAOSAppSecret; }
        public static string AzureStorageConnectionString() { return UserConfiguration().AADConfiguration.AzureStorageConnectionString; }
        public static string AADTenantId() { return UserConfiguration().AADConfiguration.AADTenantId; }
        public static string ScaleUnitId() { return UserConfiguration().ScaleUnitConfiguration.ScaleUnitId; }
        public static string HubIp() { return UserConfiguration().ScaleUnitConfiguration.HubIpAddress; }
        public static string ScaleUnitIp() { return UserConfiguration().ScaleUnitConfiguration.ScaleUnitIpAddress; }
        public static string HubAosEndpoint() { return "https://" + HubDomain(); }
        public static string ScaleUnitAosEndpoint() { return "https://" + ScaleUnitDomain(); }
        public static string ScaleUnitName() { return UserConfiguration().ScaleUnitConfiguration.ScaleUnitName; }
        public static string ScaleUnitUrlName() { return ScaleUnitDomain().Split('.')[0]; }
        public static string ServiceVolume() { return UserConfiguration().ServiceVolume; }
        public static List<ConfiguredWorkload> WorkloadList() { return UserConfiguration().Workloads; }

        public static List<string> UniqueLegalEntityValues()
        {
            List<ConfiguredWorkload> configuredWorkloads = Config.WorkloadList();
            return (List<string>)configuredWorkloads
                .Select(x => x.ConfiguredDynamicConstraintValues
                .FirstOrDefault(y => y.DomainName.Equals("LegalEntity", StringComparison.OrdinalIgnoreCase)).Value)
                .Distinct().ToList();

        }

        public static List<string> ConfiguredWorkloadInstanceIds()
        {
            List<ConfiguredWorkload> configuredWorkloads = Config.WorkloadList();
            return (List<string>)configuredWorkloads
                .Select(x => x.WorkloadInstanceId)
                .ToList();
        }

        public static List<string> SYSWorkloadInstanceIds()
        {
            string Flip(string workloadInstanceId)
            {
                if (workloadInstanceId[0] >= '0' && workloadInstanceId[0] <= '9')
                    return "a" + workloadInstanceId.Substring(1);
                else
                    return "0" + workloadInstanceId.Substring(1);
            }

            var numOfIds = UniqueLegalEntityValues().Count;

            List<ConfiguredWorkload> configuredWorkloads = Config.WorkloadList();
            return (List<string>)configuredWorkloads
                .Take(numOfIds)
                .Select(x => Flip(x.WorkloadInstanceId))
                .ToList();
        }

        public static List<string> AllWorkloadInstanceIds()
        {
            return (List<string>)SYSWorkloadInstanceIds().Concat(ConfiguredWorkloadInstanceIds()).ToList();
        }

        public static List<WorkloadInstanceIdWithName> WorkloadInstanceIdWithNameList()
        {
            List<WorkloadInstanceIdWithName> result = new List<WorkloadInstanceIdWithName>();
            List<string> sysIds = SYSWorkloadInstanceIds();
            List<ConfiguredWorkload> configuredWorkloads = Config.WorkloadList();

            foreach (string id in sysIds)
            {
                result.Add(new WorkloadInstanceIdWithName() { Name = "SYS", WorkloadInstanceId = id });
            }

            foreach (ConfiguredWorkload configuredWorkload in configuredWorkloads)
            {
                result.Add(
                    new WorkloadInstanceIdWithName()
                    { Name = configuredWorkload.Name, WorkloadInstanceId = configuredWorkload.WorkloadInstanceId }
                );
            }

            return result;

        }

        public static string HubAosResourceId()
        {
            if (String.IsNullOrEmpty(UserConfiguration().ScaleUnitConfiguration.HubAosResourceId))
                return HubAosEndpoint();

            return UserConfiguration().ScaleUnitConfiguration.HubAosResourceId;
        }

        public static string HubDomain()
        {
            return SanitizeDomain(UserConfiguration().ScaleUnitConfiguration.HubDomain);
        }

        public static string ScaleUnitDomain()
        {
            return SanitizeDomain(UserConfiguration().ScaleUnitConfiguration.ScaleUnitDomain);
        }

        public static string ScaleUnitAosResourceId()
        {
            if (String.IsNullOrEmpty(UserConfiguration().ScaleUnitConfiguration.ScaleUnitAosResourceId))
                return ScaleUnitAosEndpoint();

            return UserConfiguration().ScaleUnitConfiguration.ScaleUnitAosResourceId;
        }

        private static void Validate()
        {
            bool hasAnyError = false;

            void ValidateValue(string fieldName, string fieldValue)
            {
                if (String.IsNullOrEmpty(fieldValue))
                {
                    hasAnyError = true;
                    Console.Error.WriteLine("Missing expected field: " + fieldName);
                }
            }

            ValidateValue("AppId", AppId());
            ValidateValue("AppSecret", AppSecret());
            ValidateValue("Authority", Authority());

            ValidateValue("AzureStorageConnectionString", AzureStorageConnectionString());
            ValidateValue("HubDomain", HubDomain());
            ValidateValue("ScaleUnitDomain", ScaleUnitDomain());
            ValidateValue("ScaleUnitName", ScaleUnitName());
            ValidateValue("ScaleUnitId", ScaleUnitId());
            ValidateValue("InterAOSAppId", InterAOSAppId());
            ValidateValue("InterAOSAppSecret", InterAOSAppSecret());
            ValidateValue("ServiceVolume", ServiceVolume());

            if (hasAnyError)
            {
                Console.WriteLine("Please ensure all options are set in the UserConfig file. Operation aborted.");
                Environment.Exit(1);

            }


        }
    }

    public class CloudAndEdgeConfiguration
    {
        public string AxDbName { get; set; }
        public string AxScaleUnitDbName { get; set; }
        public AADConfiguration AADConfiguration { get; set; }
        public ScaleUnitConfiguration ScaleUnitConfiguration { get; set; }
        public LogicalEnvIdAndHash LogicalEnvIdAndHash { get; set; }
        public WorkloadInstanceIds WorkloadInstanceIds { get; set; }
        public string ServiceVolume { get; set; }
        public List<ConfiguredWorkload> Workloads { get; set; }
    }

    public class AADConfiguration
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Authority { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string AADTenantId { get; set; }
    }

    public class ScaleUnitConfiguration
    {
        public string HubDomain { get; set; }
        public string HubAosResourceId { get; set; }
        public string HubIpAddress { get; set; }
        public string ScaleUnitIpAddress { get; set; }
        public string ScaleUnitDomain { get; set; }
        public string ScaleUnitAosResourceId { get; set; }
        public string ScaleUnitEnvironmentId { get; set; }
        public string ScaleUnitName { get; set; }
        public string ScaleUnitId { get; set; }
        public string InterAOSAppId { get; set; }
        public string InterAOSAppSecret { get; set; }
    }

    public class LogicalEnvIdAndHash
    {
        public string LogicalEnvironmentId { get; set; }
        public string Hash { get; set; }
    }

    public class WorkloadInstanceIds
    {
        public string SysWorkloadInstanceId { get; set; }
        public string MesWorkloadInstanceId { get; set; }
        public string WesWorkloadInstanceId { get; set; }
    }

    public class ConfiguredWorkload
    {
        public string WorkloadInstanceId { get; set; }
        public string Name { get; set; }
        public List<ConfiguredDynamicConstraintValue> ConfiguredDynamicConstraintValues { get; set; }
    }

    public class ConfiguredDynamicConstraintValue
    {
        public string DomainName { get; set; }
        public string Value { get; set; }
    }

    public class WorkloadInstanceIdWithName
    {
        public string WorkloadInstanceId { get; set; }
        public string Name { get; set; }
    }
}
