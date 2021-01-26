using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ScaleUnitManagement.Utilities
{
    public class Config
    {
        public const int RetryCount = 5;
        public static string LogicalEnvironmentId = "82099c35-019e-45cf-9233-630ca5dd69ec";
        public static string WorkloadDefinitionHash = "c2f309922ef4fc1f1a418ae31ebf85763f8a836e355062e6489ff12e3c438c0d";
        public static string ScaleUnitEnvironmentId = "a4557a4e-6980-4e1b-b7c7-661793b4a098";


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

        public static string AppId() { return UserConfiguration().AADConfiguration.AppId; }
        public static string AppSecret() { return UserConfiguration().AADConfiguration.AppSecret; }
        public static string Authority() { return UserConfiguration().AADConfiguration.Authority; }
        public static string InterAOSAppId() { return UserConfiguration().AADConfiguration.InterAOSAppId; }
        public static string InterAOSAppSecret() { return UserConfiguration().AADConfiguration.InterAOSAppSecret; }
        public static string AADTenantId() { return UserConfiguration().AADConfiguration.AADTenantId; }
        public static List<ScaleUnitInstance> ScaleUnitInstances() { return UserConfiguration().ScaleUnitConfiguration; }
        public static List<ConfiguredWorkload> WorkloadList() { return UserConfiguration().Workloads; }

        public static ScaleUnitInstance FindScaleUnitWithId(string scaleUnitId) { return ScaleUnitInstances().Where(s => s.ScaleUnitId == scaleUnitId).SingleOrDefault(); }
        public static ScaleUnitInstance NonHubScaleUnit() { return ScaleUnitInstances().Where(s => s.ScaleUnitId != "@@").SingleOrDefault(); }
        public static ScaleUnitInstance HubScaleUnit() { return FindScaleUnitWithId("@@"); }

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

            Console.WriteLine("Validating AADConfiguration");

            ValidateValue("AppId", AppId());
            ValidateValue("AppSecret", AppSecret());
            ValidateValue("Authority", Authority());
            ValidateValue("InterAOSAppId", InterAOSAppId());
            ValidateValue("InterAOSAppSecret", InterAOSAppSecret());

            Console.WriteLine("Validating ScaleUnitConfiguration");

            if (HubScaleUnit() == null)
            {
                hasAnyError = true;
                Console.Error.WriteLine("Missing hub scale unit instance");
            }

            foreach (ScaleUnitInstance scaleUnit in ScaleUnitInstances())
            {
                Console.WriteLine($"Validating ScaleUnitInstance {scaleUnit.ScaleUnitId}");
                ValidateValue("ScaleUnitId", scaleUnit.ScaleUnitId);
                ValidateValue("Domain", scaleUnit.Domain);
                ValidateValue("IpAddress", scaleUnit.IpAddress);
                ValidateValue("AxDbName", scaleUnit.AxDbName);
                ValidateValue("ScaleUnitName", scaleUnit.ScaleUnitName);
                ValidateValue("ServiceVolume", scaleUnit.ServiceVolume);

                if (scaleUnit.EnvironmentType == ScaleUnitManagement.Utilities.EnvironmentType.Unknown)
                {
                    hasAnyError = true;
                    Console.Error.WriteLine("EnvironmentType is not valid. The following values are supported (see documentation for help): ");

                    foreach (ScaleUnitManagement.Utilities.EnvironmentType val in Enum.GetValues(typeof(ScaleUnitManagement.Utilities.EnvironmentType)))
                    {
                        if (val != ScaleUnitManagement.Utilities.EnvironmentType.Unknown)
                            Console.WriteLine(val);
                    }
                }


                if (scaleUnit.EnvironmentType == ScaleUnitManagement.Utilities.EnvironmentType.VHD)
                {
                    ValidateValue("AzureStorageConnectionString", scaleUnit.AzureStorageConnectionString);
                }
            }

            Console.WriteLine("Validating Workloads");

            foreach (ConfiguredWorkload workload in WorkloadList())
            {
                ValidateValue("ScaleUnitId", workload.ScaleUnitId);
            }

            if (hasAnyError)
            {
                Console.WriteLine("Please ensure all options are set in the UserConfig file. Operation aborted.");
                Environment.Exit(1);

            }
        }
    }

    public class CloudAndEdgeConfiguration
    {
        public AADConfiguration AADConfiguration { get; set; }
        public List<ScaleUnitInstance> ScaleUnitConfiguration { get; set; }
        public List<ConfiguredWorkload> Workloads { get; set; }
    }

    public class AADConfiguration
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Authority { get; set; }
        public string AADTenantId { get; set; }
        public string InterAOSAppId { get; set; }
        public string InterAOSAppSecret { get; set; }
    }

    public class ScaleUnitInstance : IComparable<ScaleUnitInstance>
    {
        public string Domain { get; set; }
        public string IpAddress { get; set; }
        public string AzureStorageConnectionString { get; set; }
        public string AxDbName { get; set; }
        public string ScaleUnitName { get; set; }
        public string ScaleUnitId { get; set; }
        public EnvironmentType EnvironmentType { get; set; }
        public string ServiceVolume { get; set; }

        public string DomainSafe()
        {
            // trim surrounding whitespace
            string domainName = Domain.Trim();

            // remove https?://
            domainName = Regex.Replace(domainName, @"^(http(s)?://)?", string.Empty);

            // * ensure no "/" suffix
            domainName = domainName.TrimEnd('/');

            return domainName;
        }

        public string ResourceId() { return Endpoint(); }
        public string Endpoint() { return "https://" + DomainSafe(); }
        public string ScaleUnitUrlName() { return DomainSafe().Split('.')[0]; }

        public string ConfigEncryptorExePath() { return $@"{ServiceVolume}\AOSService\webroot\bin\Microsoft.Dynamics.AX.Framework.ConfigEncryptor.exe"; }
        public string WebConfigPath() { return $@"{ServiceVolume}\AOSService\webroot\web.config"; }
        public string WifServicesConfigPath() { return $@"{ServiceVolume}\AOSService\webroot\wif.services.config"; }

        public int CompareTo(ScaleUnitInstance other)
        {
            if (this.ScaleUnitId == "@@")
                return -1;
            if (other.ScaleUnitId == "@@")
                return 1;

            return this.ScaleUnitId.CompareTo(other.ScaleUnitId);
        }
    }

    public class ConfiguredWorkload
    {
        public string WorkloadInstanceId { get; set; }
        public string Name { get; set; }
        public string ScaleUnitId { get; set; }
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
