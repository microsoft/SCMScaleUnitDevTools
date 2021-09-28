using System;
using System.Linq;
using System.Xml.Linq;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class WebConfig : IDisposable
    {
        private readonly XDocument doc;
        private readonly XElement appSettingsElement;
        private readonly string webConfigPath;
        private readonly string configEncryptorExePath;

        public WebConfig()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            webConfigPath = scaleUnit.WebConfigPath();
            configEncryptorExePath = scaleUnit.ConfigEncryptorExePath();

            doc = XDocument.Load(webConfigPath);

            appSettingsElement = doc.Descendants()
                    .Where(x => (string)x.Name.LocalName == "appSettings")
                    .FirstOrDefault();
        }

        public void AddKey(string keyName, string keyValue)
        {
            XElement element = appSettingsElement.Descendants()
                    .Where(x => (string)x.Attribute("key") == keyName)
                    .FirstOrDefault();

            if (element != null)
            {
                element.SetAttributeValue("value", keyValue);
            }

            else
            {
                var addElement = new XElement("add");

                addElement.Add(new XAttribute("key", keyName));
                addElement.Add(new XAttribute("value", keyValue));

                appSettingsElement.Add(addElement);
            }
        }

        public void UpdateXElementIfExists(string key, string value)
        {
            XElement element = doc.Descendants()
                    .Where(x => (string)x.Attribute("key") == key)
                    .FirstOrDefault();

            if (element is null)
            {
                return;
            }

            element.SetAttributeValue("value", value);
        }

        public string GetXElementValue(string key)
        {
            XElement element = doc.Descendants()
                .Where(x => (string)x.Attribute("key") == key)
                .FirstOrDefault();

            return element?.Attribute("value")?.Value;
        }

        public void Dispose()
        {
            doc.Save(webConfigPath);

            //Encrypt config file
            var ce = new CommandExecutor(configEncryptorExePath, "-encrypt " + webConfigPath);
            ce.RunCommand();
        }
    }
}
