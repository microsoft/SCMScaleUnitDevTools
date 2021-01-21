using System;
using System.Linq;
using Microsoft.Web.Administration;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ConfigureScaleUnit : IScaleUnitStep
    {
        public string Label()
        {
            return "Configure Scale unit";
        }

        public float Priority()
        {
            return 2F;
        }

        public void Run()
        {
            IISAdministrationHelper.CreateSite(
                siteName: Config.ScaleUnitAppPoolName,
                siteRoot: Config.ScaleUnitSiteRoot,
                bindingInformation: "127.0.0.11:443:" + Config.ScaleUnitDomain(),
                certSubject: Config.ScaleUnitDomain(),
                appPoolName: Config.ScaleUnitAppPoolName);

            using (var hosts = new Hosts())
            {
                hosts.AddMapping(Config.ScaleUnitIp(), Config.ScaleUnitDomain());
            }

            using (var webConfig = new WebConfig(Config.ScaleUnitWebConfigPath))
            {
                if (!string.IsNullOrEmpty(Config.AADTenantId()))
                {
                    webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());
                }

                webConfig.UpdateXElement("AzureStorage.StorageConnectionString", Config.StorageEmulatorConnectionString);
                webConfig.UpdateXElement("Infrastructure.StartStorageEmulator", "true");


                webConfig.UpdateXElement("Infrastructure.FullyQualifiedDomainName", Config.ScaleUnitDomain());
                webConfig.UpdateXElement("Infrastructure.HostName", Config.ScaleUnitDomain());
                webConfig.UpdateXElement("Infrastructure.HostedServiceName", Config.ScaleUnitUrlName());

                string scaleUnitUrl = Config.ScaleUnitAosEndpoint() + "/";
                webConfig.UpdateXElement("Infrastructure.HostUrl", scaleUnitUrl);
                webConfig.UpdateXElement("Infrastructure.SoapServicesUrl", scaleUnitUrl);
                webConfig.UpdateXElement("DataAccess.Database", "AxDbEmpty");

                webConfig.AddKey("ScaleUnit.InstanceID", Config.ScaleUnitId());
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            new WifServiceConfig(Config.ScaleUnitWifServicesConfigPath).Update();

            CreateScaleUnitBatch();
        }

        private void CreateScaleUnitBatch()
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd = $@"
                .$env:systemroot\system32\sc.exe delete {Config.ScaleUnitBatchName}; 
                $secpasswd = (new-object System.Security.SecureString);
                $creds = New-Object System.Management.Automation.PSCredential ('NT AUTHORITY\NETWORK SERVICE', $secpasswd);
                New-Service -Name '{Config.ScaleUnitBatchName}' -BinaryPathName '{Config.DynamicsBatchExePath} -service {Config.ScaleUnitWebConfigPath}' -credential $creds -startupType Manual;
            ";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
