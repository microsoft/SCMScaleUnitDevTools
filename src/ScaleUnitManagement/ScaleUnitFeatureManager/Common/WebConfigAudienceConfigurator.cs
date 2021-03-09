using System.Collections.Generic;
using System.Linq;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public static class WebConfigAudienceConfigurator
    {
        public static void AddValidAudiences(WebConfig webConfig)
        {
            const string validAudienceConfigKey = "Aad.AADValidAudience";

            string[] aadValidAudiences = webConfig.GetXElementValue(validAudienceConfigKey)?.Split(';');

            var validAudiencesToAdd = new List<string>();

            if (!string.IsNullOrWhiteSpace(Config.AppResourceId())
                && !aadValidAudiences.Contains(Config.AppResourceId()))
            {
                validAudiencesToAdd.Add(Config.AppResourceId());
            }

            if (!string.IsNullOrWhiteSpace(Config.InterAOSAppResourceId())
                && !aadValidAudiences.Contains(Config.InterAOSAppResourceId()))
            {
                validAudiencesToAdd.Add(Config.InterAOSAppResourceId());
            }

            if (validAudiencesToAdd.Any())
            {
                webConfig.UpdateXElement(validAudienceConfigKey, string.Join(";", aadValidAudiences.Concat(validAudiencesToAdd)));
            }
        }
    }
}
