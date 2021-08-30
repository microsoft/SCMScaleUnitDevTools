using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public static class SharedWebConfig
    {
        public static void Configure(WebConfig webConfig, bool isScaleUnitFeatureEnabled = true)
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            if (scaleUnit.EnvironmentType == EnvironmentType.VHD || Config.UseSingleOneBox())
            {
                if (!String.IsNullOrEmpty(scaleUnit.AzureStorageConnectionString))
                    webConfig.UpdateXElementIfExists("AzureStorage.StorageConnectionString", scaleUnit.AzureStorageConnectionString);
            }

            webConfig.AddKey("ScaleUnit.InstanceID", isScaleUnitFeatureEnabled ? scaleUnit.ScaleUnitId : "");
            webConfig.AddKey("ScaleUnit.Enabled", isScaleUnitFeatureEnabled.ToString().ToLower());
            webConfig.AddKey("DbSync.TriggersEnabled", "true");
        }
    }
}
