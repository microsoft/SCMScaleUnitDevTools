using System;
using System.IO;
using System.Linq;
using System.Reflection;
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

        public void UpdateXElementIfExists(string key, string value)
        {
            XElement element = GetElementWithAttribute(doc, key);

            if (element is null)
            {
                return;
            }

            element.SetAttributeValue("value", value);
        }

        public string GetXElementValue(string key)
        {
            XElement element;
            if (key.Equals("AzureStorage.StorageConnectionString"))
            {
                element = GetDecryptedElementWithAttribute(key);
            }
            else
            {
                element = GetElementWithAttribute(doc, key);
            }

            return element?.Attribute("value")?.Value;
        }

        private XElement GetDecryptedElementWithAttribute(string key)
        {
            string configPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            string tmpDirectory = $"{configPath}/tmp";
            string decryptPath = $"{tmpDirectory}/web.config";

            if (!Directory.Exists(tmpDirectory))
            {
                Directory.CreateDirectory(tmpDirectory);
            }

            XElement element = null;
            try
            {
                File.Copy(webConfigPath, decryptPath);
                var ce = new CommandExecutor(configEncryptorExePath, "-decrypt " + decryptPath);
                ce.RunCommand();

                XDocument decryptedDoc = XDocument.Load(decryptPath);
                element = GetElementWithAttribute(decryptedDoc, key);
            }
            finally
            {
                File.Delete(decryptPath);
            }

            return element;
        }

        private XElement GetElementWithAttribute(XDocument document, string key)
        {
            return document.Descendants()
                .Where(x => (string)x.Attribute("key") == key)
                .FirstOrDefault();
        }

        public void Dispose()
        {
            doc.Save(webConfigPath);

            //Encrypt config file
            CommandExecutor ce = new CommandExecutor(configEncryptorExePath, "-encrypt " + webConfigPath);
            ce.RunCommand();
        }
    }
}
