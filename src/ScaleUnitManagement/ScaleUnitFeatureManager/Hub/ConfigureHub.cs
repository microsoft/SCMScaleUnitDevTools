using System;
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
            using (var webConfig = new WebConfig())
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


            using (ServerManager manager = new ServerManager())
            {
                Site site = manager.Sites["AOSService"];
                site.Bindings.Clear();
                site.Bindings.Add("127.0.0.10:443:" + Config.HubDomain(), "https");

                manager.CommitChanges();
            }
        }
    }
}
