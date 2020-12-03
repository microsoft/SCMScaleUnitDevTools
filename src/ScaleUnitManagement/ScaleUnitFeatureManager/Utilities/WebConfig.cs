using System;
using System.Linq;
using System.Xml.Linq;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class WebConfig : IDisposable
    {
        private XDocument doc;
        private XElement appSettingsElement;

        public WebConfig()
        {
            doc = XDocument.Load(Config.WebConfigPath);

            appSettingsElement = doc.Descendants()
                    .Where(x => (string)x.Name.LocalName == "appSettings")
                    .FirstOrDefault();
        }

        public void AddKey(string keyName, string keyValue)
        {
            var element = appSettingsElement.Descendants()
                    .Where(x => (string)x.Attribute("key") == keyName)
                    .FirstOrDefault();

            if (element != null)
            {
                element.SetAttributeValue("value", keyValue);
            }

            else
            {
                XElement addElement = new XElement("add");

                addElement.Add(new XAttribute("key", keyName));
                addElement.Add(new XAttribute("value", keyValue));

                appSettingsElement.Add(addElement);
            }
        }

        public void UpdateXElement(string key, string value)
        {
            var element = doc.Descendants()
                    .Where(x => (string)x.Attribute("key") == key)
                    .FirstOrDefault();
            element.SetAttributeValue("value", value);
        }

        public void Dispose()
        {
            doc.Save(Config.WebConfigPath);

            //Encrypt config file
            string encryptConfigFileCommand = Config.ConfigEncryptorExePath + " -encrypt " + Config.WebConfigPath;

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(encryptConfigFileCommand);
        }
    }
}
