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
                webConfig.UpdateXElement("DataAccess.Database", "AxDbEmpty");

                webConfig.AddKey("ScaleUnit.InstanceID", Config.ScaleUnitId());
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            WifServiceConfig.Update();

            using (var hosts = new Hosts())
            {
                hosts.AddMapping(Config.ScaleUnitIp(), Config.ScaleUnitDomain());
            }

            using (ServerManager manager = new ServerManager())
            {
                var siteName = "AOSServiceScaleUnit";
                Site site = manager.Sites.FirstOrDefault((s) => s.Name.Equals(siteName));

                if (site == null)
                {
                    manager.Sites.Add(siteName, @"C:\AOSService\webrootspoke", 443);
                }

                site.Bindings.Clear();
                site.Bindings.Add("127.0.0.11:443:" + Config.ScaleUnitDomain(), "https");

                manager.CommitChanges();
            }
        }
    }
}
