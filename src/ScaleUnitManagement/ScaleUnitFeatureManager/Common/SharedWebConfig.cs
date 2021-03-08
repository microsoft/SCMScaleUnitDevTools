using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public static class SharedWebConfig
    {
        public static async Task Configure(WebConfig webConfig)
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            if (scaleUnit.EnvironmentType == EnvironmentType.VHD || Config.UseSingleOneBox())
            {
                if (!string.IsNullOrEmpty(Config.AADTenantId()))
                {
                    webConfig.UpdateXElement("Aad.AADTenantId", Config.AADTenantId());

                    try
                    {
                        string tenantDomainGUID = await OAuthHelper.GetTidClaim(Config.AADTenantId(), Config.AppId(), Config.AppSecret(), Config.HubScaleUnit().ResourceId());

                        if (!string.IsNullOrWhiteSpace(tenantDomainGUID))
                            webConfig.UpdateXElement("Aad.TenantDomainGUID", tenantDomainGUID);
                    }
                    catch (Exception)
                    {
                        Console.Error.WriteLine($"Unable to get tenant ID for {Config.AADTenantId()}, skipping setting Aad.AADTenantDomainGUID in Web.config\n");
                        throw;
                    }
                }

                if (!String.IsNullOrEmpty(scaleUnit.AzureStorageConnectionString))
                    webConfig.UpdateXElement("AzureStorage.StorageConnectionString", scaleUnit.AzureStorageConnectionString);
            }

            webConfig.AddKey("ScaleUnit.InstanceID", scaleUnit.ScaleUnitId);
            webConfig.AddKey("ScaleUnit.Enabled", "true");
            webConfig.AddKey("DbSync.TriggersEnabled", "true");
        }
    }
}