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
        }
    }
}
