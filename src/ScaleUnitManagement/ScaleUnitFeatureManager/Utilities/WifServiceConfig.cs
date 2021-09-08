using System.Xml;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class WifServiceConfig
    {
        public static void Update()
        {
            var scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            var wifDoc = new XmlDocument();
            wifDoc.Load(scaleUnit.WifServicesConfigPath());

            // Update urls in wif.services.config
            var cookieHandlerNode = wifDoc.SelectSingleNode("/system.identityModel.services/federationConfiguration/cookieHandler");
            var attrColl = cookieHandlerNode.Attributes;
            var attr = (XmlAttribute)attrColl.GetNamedItem("domain");
            attr.Value = scaleUnit.DomainSafe();

            wifDoc.Save(scaleUnit.WifServicesConfigPath());
        }
    }
}
