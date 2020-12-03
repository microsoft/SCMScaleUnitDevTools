using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class ConfigureHub : HubStep
    {
        public override string Label()
        {
            return "Configure Hub";
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
                        webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());

                    if (!String.IsNullOrEmpty(Config.AzureStorageConnectionString()))
                        webConfig.UpdateXElement("AzureStorage.StorageConnectionString", Config.AzureStorageConnectionString());

                    webConfig.UpdateXElement("Infrastructure.StartStorageEmulator", "false");
                }

                webConfig.AddKey("ScaleUnit.InstanceID", "@@");
                webConfig.AddKey("ScaleUnit.Enabled", "true");
                webConfig.AddKey("DbSync.TriggersEnabled", "true");
            }
        }
    }
}
