using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace ScaleUnitManagement.Utilities
{
    class LoadUserConfig
    {
        private static void TrimChildNodes(XmlNodeList nodes)
        {
            foreach (XmlNode node in nodes)
            {
                if (!String.IsNullOrEmpty(node.InnerXml))
                {
                    node.InnerXml = node.InnerXml.Trim();
                }

                TrimChildNodes(node.ChildNodes);
            }
        }

        public static CloudAndEdgeConfiguration LoadConfig()
        {
            try
            {
                var doc = new XmlDocument();
                string exePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                string userConfigPath = Path.Combine(exePath, "UserConfig.xml");

                if (!File.Exists(userConfigPath))
                {
                    throw new FileNotFoundException($"Missing UserConfig.xml, should be located at {userConfigPath}. Rename the sample file and fill in values for your Scale Unit project.", fileName: userConfigPath);
                }
                doc.Load(userConfigPath);

                TrimChildNodes(doc.ChildNodes);

                var serializer = new XmlSerializer(typeof(CloudAndEdgeConfiguration), new XmlRootAttribute("CloudAndEdgeConfiguration"));

                var stringReader = new StringReader(doc.InnerXml);

                return (CloudAndEdgeConfiguration)serializer.Deserialize(XmlReader.Create(stringReader));
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                throw;
            }
        }
    }
}
