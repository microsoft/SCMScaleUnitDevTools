using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    internal static class IISSiteHelper
    {
        internal static void CreateSite(string siteName, string siteRoot,
            string bindingInformation, string certSubject, string appPoolName)
        {
            using (ServerManager manager = new ServerManager())
            {
                Site site = manager.Sites.FirstOrDefault((s) => s.Name.Equals(siteName));

                if (site != null)
                {
                    manager.Sites.Remove(site);
                }

                site = manager.Sites.Add(siteName, siteRoot, 443);
                site.Applications[0].ApplicationPoolName = appPoolName;

                site.Bindings.Clear();
                site.Bindings.Add(bindingInformation,
                    CertificateStoreHelper.GetCertificateFromLocalMachineStore(certSubject),
                    "MY");

                manager.CommitChanges();
            }
        }
    }
}
