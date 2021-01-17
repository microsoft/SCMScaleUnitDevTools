using System;
using System.Linq;
using Microsoft.Web.Administration;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class ConfigureHub : IHubStep
    {
        public string Label()
        {
            return "Configure Hub";
        }

        public float Priority()
        {
            return 2F;
        }

        public void Run()
        {
            using (var webConfig = new WebConfig(Config.HubWebConfigPath))
            {
                if (!String.IsNullOrEmpty(Config.AADTenantId()))
                    webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());

                webConfig.AddKey("ScaleUnit.InstanceID", "@@");
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            using (var hosts = new Hosts())
            {
                hosts.AddMapping(Config.HubIp(), Config.HubDomain());
            }

            IISSiteHelper.CreateSite(
                siteName: "AOSService",
                siteRoot: @"C:\AOSService\webroot",
                bindingInformation: "127.0.0.10:443:" + Config.HubDomain(),
                certSubject: "*.cloud.onebox.dynamics.com",
                appPoolName: "AOSService");
        }
    }
}
