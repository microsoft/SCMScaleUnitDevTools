using System;
using Microsoft.Web.Administration;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ConfigureScaleUnit : ScaleUnitStep
    {
        public override string Label()
        {
            return "Configure Scale unit";
        }

        public override float Priority()
        {
            return 2F;
        }

        public override void Run()
        {
            using (var webConfig = new WebConfig())
            {
                if (Config.EnvironmentType() == EnvironmentType.VHD)
                {
                    if (!String.IsNullOrEmpty(Config.AADTenantId()))
                    {
                        webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());
                    }

                    if (!String.IsNullOrEmpty(Config.AzureStorageConnectionString()))
                        webConfig.UpdateXElement("AzureStorage.StorageConnectionString", Config.AzureStorageConnectionString());

                    webConfig.UpdateXElement("Infrastructure.FullyQualifiedDomainName", Config.ScaleUnitDomain());
                    webConfig.UpdateXElement("Infrastructure.HostName", Config.ScaleUnitDomain());
                    webConfig.UpdateXElement("Infrastructure.HostedServiceName", Config.ScaleUnitUrlName());

                    string scaleUnitUrl = Config.ScaleUnitAosEndpoint() + "/";
                    webConfig.UpdateXElement("Infrastructure.HostUrl", scaleUnitUrl);
                    webConfig.UpdateXElement("Infrastructure.SoapServicesUrl", scaleUnitUrl);
                }

                webConfig.AddKey("ScaleUnit.InstanceID", Config.ScaleUnitId());
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            WifServiceConfig.Update();

            if (Config.EnvironmentType() == EnvironmentType.VHD)
            {
                // Update hosts file
                using (var hosts = new Hosts())
                {
                    hosts.AddMapping("127.0.0.1", Config.ScaleUnitDomain());
                    hosts.AddMapping(Config.HubIp(), Config.HubDomain());
                }

                // Configure IIS binding
                using (ServerManager manager = new ServerManager())
                {
                    Site site = manager.Sites["AOSService"];
                    site.Bindings.Clear();
                    site.Bindings.Add("*:443:" + Config.ScaleUnitDomain(), "https");

                    manager.CommitChanges();
                }
            }
        }
    }
}
