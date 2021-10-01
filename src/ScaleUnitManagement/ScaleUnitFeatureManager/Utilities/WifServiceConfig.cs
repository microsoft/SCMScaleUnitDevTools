using System.Xml;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class WifServiceConfig
    {
        public static void Update()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            var wifDoc = new XmlDocument();
            wifDoc.Load(scaleUnit.WifServicesConfigPath());

            // Update urls in wif.services.config
            XmlNode cookieHandlerNode = wifDoc.SelectSingleNode("/system.identityModel.services/federationConfiguration/cookieHandler");
            XmlAttributeCollection attrColl = cookieHandlerNode.Attributes;
            var attr = (XmlAttribute)attrColl.GetNamedItem("domain");
            attr.Value = scaleUnit.DomainSafe();

            wifDoc.Save(scaleUnit.WifServicesConfigPath());
        }
    }
}
