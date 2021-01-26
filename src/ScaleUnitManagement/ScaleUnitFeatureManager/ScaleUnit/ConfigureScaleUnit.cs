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
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            using (var webConfig = new WebConfig())
            {
                if (scaleUnit.EnvironmentType == EnvironmentType.VHD)
                {
                    if (!String.IsNullOrEmpty(Config.AADTenantId()))
                    {
                        webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());
                    }

                    if (!String.IsNullOrEmpty(scaleUnit.AzureStorageConnectionString))
                        webConfig.UpdateXElement("AzureStorage.StorageConnectionString", scaleUnit.AzureStorageConnectionString);

                    webConfig.UpdateXElement("Infrastructure.FullyQualifiedDomainName", scaleUnit.DomainSafe());
                    webConfig.UpdateXElement("Infrastructure.HostName", scaleUnit.DomainSafe());
                    webConfig.UpdateXElement("Infrastructure.HostedServiceName", scaleUnit.ScaleUnitUrlName());

                    string scaleUnitUrl = scaleUnit.Endpoint() + "/";
                    webConfig.UpdateXElement("Infrastructure.HostUrl", scaleUnitUrl);
                    webConfig.UpdateXElement("Infrastructure.SoapServicesUrl", scaleUnitUrl);
                }

                webConfig.AddKey("ScaleUnit.InstanceID", scaleUnit.ScaleUnitId);
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            WifServiceConfig.Update();

            if (scaleUnit.EnvironmentType == EnvironmentType.VHD)
            {
                // Update hosts file
                using (var hosts = new Hosts())
                {
                    hosts.AddMapping("127.0.0.1", scaleUnit.DomainSafe());
                    hosts.AddMapping(Config.HubScaleUnit().IpAddress, Config.HubScaleUnit().DomainSafe());
                }

                // Configure IIS binding
                using (ServerManager manager = new ServerManager())
                {
                    Site site = manager.Sites["AOSService"];
                    site.Bindings.Clear();
                    site.Bindings.Add("*:443:" + scaleUnit.DomainSafe(), "https");

                    manager.CommitChanges();
                }
            }
        }
    }
}
