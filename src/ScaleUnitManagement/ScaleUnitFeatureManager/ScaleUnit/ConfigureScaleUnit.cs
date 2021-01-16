using System;
using Microsoft.Web.Administration;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ConfigureScaleUnit : IScaleUnitStep
    {
        public  string Label()
        {
            return "Configure Scale unit";
        }

        public  float Priority()
        {
            return 2F;
        }

        public  void Run()
        {
            using (var webConfig = new WebConfig())
            {
                if (!String.IsNullOrEmpty(Config.AADTenantId()))
                {
                    webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());
                }

                webConfig.UpdateXElement("Infrastructure.FullyQualifiedDomainName", Config.ScaleUnitDomain());
                webConfig.UpdateXElement("Infrastructure.HostName", Config.ScaleUnitDomain());
                webConfig.UpdateXElement("Infrastructure.HostedServiceName", Config.ScaleUnitUrlName());

                string scaleUnitUrl = Config.ScaleUnitAosEndpoint() + "/";
                webConfig.UpdateXElement("Infrastructure.HostUrl", scaleUnitUrl);
                webConfig.UpdateXElement("Infrastructure.SoapServicesUrl", scaleUnitUrl);

                webConfig.AddKey("ScaleUnit.InstanceID", Config.ScaleUnitId());
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            WifServiceConfig.Update();


            // TODO: Fix this part. Share between hub and spoke and fix mapping and binding for both. 
            // Update hosts file
            using (var hosts = new Hosts())
            {
                hosts.AddMapping(Config.HubIp(), Config.HubDomain());
                hosts.AddMapping(Config.ScaleUnitIp(), Config.ScaleUnitDomain());
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
