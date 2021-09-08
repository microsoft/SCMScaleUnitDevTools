using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class Hosts : IDisposable
    {
        private readonly List<string> hostsContent;
        private static readonly string hostsFilePath = @"C:\Windows\System32\Drivers\etc\hosts";

        public Hosts()
        {
            var lines = System.IO.File.ReadAllLines(hostsFilePath);
            hostsContent = lines.ToList<string>();
        }

        public void AddMapping(string ip, string domain)
        {
            var lineCount = hostsContent.Count;
            for (var i = 0; i < lineCount;)
            {
                if (LineResolvesDomain(hostsContent[i], domain))
                {
                    hostsContent.RemoveAt(i);
                    lineCount--;
                }
                else
                {
                    i++;
                }
            }

            hostsContent.Add(CreateMapping(ip, domain));
        }

        public bool LineResolvesDomain(string line, string domain)
        {
            var parts = Regex.Split(line, "(\t+)");
            return parts.Length == 3 && parts[2] == domain;
        }

        public string CreateMapping(string ip, string domain)
        {
            return ip + "\t" + domain;
        }

        public void Dispose()
        {
            System.IO.File.WriteAllLines(hostsFilePath, hostsContent);
        }
    }
}
