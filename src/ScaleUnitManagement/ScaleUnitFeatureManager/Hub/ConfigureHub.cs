using System;
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
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            using (var webConfig = new WebConfig())
            {
                if (scaleUnit.EnvironmentType == EnvironmentType.VHD)
                {
                    if (!String.IsNullOrEmpty(Config.AADTenantId()))
                        webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());

                    if (!String.IsNullOrEmpty(scaleUnit.AzureStorageConnectionString))
                        webConfig.UpdateXElement("AzureStorage.StorageConnectionString", scaleUnit.AzureStorageConnectionString);

                    webConfig.UpdateXElement("Infrastructure.StartStorageEmulator", "false");
                }

                webConfig.AddKey("ScaleUnit.InstanceID", scaleUnit.ScaleUnitId);
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }

            if (Config.UseSingleOneBox())
            {
                // Update hosts file
                using (var hosts = new Hosts())
                {
                    hosts.AddMapping(scaleUnit.IpAddress, scaleUnit.DomainSafe());
                }

                // Update IIS binding
                IISAdministrationHelper.CreateSite(
                    siteName: scaleUnit.SiteName(),
                    siteRoot: scaleUnit.SiteRoot(),
                    bindingInformation: scaleUnit.IpAddress + ":443:" + scaleUnit.DomainSafe(),
                    certSubject: scaleUnit.DomainSafe(),
                    appPoolName: scaleUnit.AppPoolName());
            }
        }
    }
}
