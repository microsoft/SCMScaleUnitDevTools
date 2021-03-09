using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public static class SharedWebConfig
    {
        public static void Configure(WebConfig webConfig)
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            if (scaleUnit.EnvironmentType == EnvironmentType.VHD || Config.UseSingleOneBox())
            {
                if (!String.IsNullOrEmpty(scaleUnit.AzureStorageConnectionString))
                    webConfig.UpdateXElement("AzureStorage.StorageConnectionString", scaleUnit.AzureStorageConnectionString);
            }

            webConfig.AddKey("ScaleUnit.InstanceID", scaleUnit.ScaleUnitId);
            webConfig.AddKey("ScaleUnit.Enabled", "true");
            webConfig.AddKey("DbSync.TriggersEnabled", "true");
        }
    }
}
