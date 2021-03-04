using System;
using System.Collections.Generic;
using System.Linq;
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

        public static bool UseSingleOneBox() { return UserConfiguration().UseSingleOneBox; }
        public static string AADTenantId() { return UserConfiguration().AADTenantId; }
        public static string InterAOSAppId() { return UserConfiguration().InterAOSAADConfiguration.AppId; }
        public static string InterAOSAppSecret() { return UserConfiguration().InterAOSAADConfiguration.AppSecret; }
        public static string InterAOSAuthority() { return UserConfiguration().InterAOSAADConfiguration.Authority; }
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

            Console.WriteLine("Validating InterAOS AAD Configuration");

            ValidateValue("InterAOSAppId", InterAOSAppId());
            ValidateValue("InterAOSAppSecret", InterAOSAppSecret());
            ValidateValue("InterAOSAuthority", InterAOSAuthority());

            Console.WriteLine("Validating ScaleUnitConfiguration");

            if (HubScaleUnit() == null)
            {
                hasAnyError = true;
                Console.Error.WriteLine("Missing hub scale unit instance. A scale unit instance with scale unit id @@ must exist.");
            }

            foreach (ScaleUnitInstance scaleUnit in ScaleUnitInstances())
            {
                Console.WriteLine($"Validating ScaleUnitInstance {scaleUnit.ScaleUnitId}");
                ValidateValue("ScaleUnitId", scaleUnit.ScaleUnitId);
                ValidateValue("Domain", scaleUnit.Domain);
                ValidateValue("AxDbName", scaleUnit.AxDbName);
                ValidateValue("ScaleUnitName", scaleUnit.ScaleUnitName);
                ValidateValue("ServiceVolume", scaleUnit.ServiceVolume);
                ValidateValue("AppId", scaleUnit.AuthConfiguration.AppId);
                ValidateValue("AppSecret", scaleUnit.AuthConfiguration.AppSecret);
                ValidateValue("Authority", scaleUnit.AuthConfiguration.Authority);

                if (Config.ScaleUnitInstances().Where(s => s.ScaleUnitId == scaleUnit.ScaleUnitId).Count() != 1)
                    Console.Error.WriteLine("ScaleUnitId is not unique");

                if (scaleUnit.EnvironmentType == EnvironmentType.Unknown)
                {
                    hasAnyError = true;
                    Console.Error.WriteLine("EnvironmentType is not valid. The following values are supported (see documentation for help): ");

                    foreach (EnvironmentType val in Enum.GetValues(typeof(EnvironmentType)))
                    {
                        if (val != EnvironmentType.Unknown)
                            Console.WriteLine(val);
                    }
                }

                if (scaleUnit.EnvironmentType == EnvironmentType.VHD)
                {
                    if (scaleUnit.IsHub())
                        ValidateValue("IpAddress", scaleUnit.IpAddress);

                    ValidateValue("AzureStorageConnectionString", scaleUnit.AzureStorageConnectionString);
                }

                if (Config.UseSingleOneBox())
                {
                    ValidateValue("IpAddress", scaleUnit.IpAddress);

                    if (ScaleUnitInstances().Where(s => s.ScaleUnitId != scaleUnit.ScaleUnitId).Any(s => s.IpAddress == scaleUnit.IpAddress))
                    {
                        hasAnyError = true;
                        Console.Error.WriteLine("The Hub and Scale Unit IpAddresses must be distinct values when using a single one-box setup.");
                    }

                    if (!scaleUnit.IsHub())
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
        public bool UseSingleOneBox { get; set; }
        public string AADTenantId { get; set; }
        public AuthConfiguration InterAOSAADConfiguration { get; set; }
        public List<ScaleUnitInstance> ScaleUnitConfiguration { get; set; }
        public List<ConfiguredWorkload> Workloads { get; set; }
    }

    public class AuthConfiguration
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string Authority { get; set; }
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
        public AuthConfiguration AuthConfiguration { get; set; }

        public string AOSRequestPathPrefix()
        {
            return EnvironmentType == EnvironmentType.LBD ? "namespaces/AXSF/" : "";
        }

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

        public string BatchServiceName()
        {
            if (!Config.UseSingleOneBox() || this.IsHub())
                return "DynamicsAxBatch";

            return $"DynamicsAx{ScaleUnitUrlName()}Batch";
        }

        public string AppPoolName()
        {
            if (!Config.UseSingleOneBox() || this.IsHub())
                return "AOSService";

            return $"AOSService{ScaleUnitUrlName()}";
        }

        public string SiteName() { return AppPoolName(); }

        public string SiteRoot()
        {
            if (!Config.UseSingleOneBox() || this.IsHub())
                return $@"{ServiceVolume}\AOSService\webroot";

            return $@"{ServiceVolume}\AOSService\webroot{ScaleUnitUrlName()}";
        }

        public string ConfigEncryptorExePath() { return $@"{SiteRoot()}\bin\Microsoft.Dynamics.AX.Framework.ConfigEncryptor.exe"; }
        public string WebConfigPath() { return $@"{SiteRoot()}\web.config"; }
        public string WifServicesConfigPath() { return $@"{SiteRoot()}\wif.services.config"; }
        public string DynamicsBatchExePath() { return $@"{SiteRoot()}\bin\Batch.exe"; }

        public int CompareTo(ScaleUnitInstance other)
        {
            if (this.IsHub())
                return -1;
            if (other.IsHub())
                return 1;

            return this.ScaleUnitId.CompareTo(other.ScaleUnitId);
        }

        public bool IsHub()
        {
            return ScaleUnitId == "@@";
        }

        public string PrintableName()
        {
            if (IsHub())
                return $"Hub: {ScaleUnitName} ({ScaleUnitId})";

            return $"Scale unit: {ScaleUnitName} ({ScaleUnitId})";
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
