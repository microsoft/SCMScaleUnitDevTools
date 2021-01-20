using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft;
using Microsoft.Web.Administration;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    internal static class IISAdministrationHelper
    {
        internal static void CreateSite(string siteName, string siteRoot,
            string bindingInformation, string certSubject, string appPoolName)
        {
            Requires.NotNull(siteName, nameof(siteName));
            Requires.NotNull(siteRoot, nameof(siteRoot));
            Requires.NotNull(bindingInformation, nameof(bindingInformation));
            Requires.NotNull(certSubject, nameof(certSubject));
            Requires.NotNull(appPoolName, nameof(appPoolName));

            CreateAppPool(appPoolName);

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
                    CertificateStoreHelper.PersonalCertificateStoreName);

                manager.CommitChanges();
            }
        }

        private static void CreateAppPool(string appPoolName)
        {
            // we are assuming HubAppPool is already and doesnt need changes. 
            if (appPoolName.Equals(Config.HubAppPoolName))
            {
                return;
            }

            CloneScaleUnitWebRoot();

            using (ServerManager manager = new ServerManager())
            {
                var hubAppPool = manager.ApplicationPools.FirstOrDefault((p) => p.Name.Equals(Config.HubAppPoolName));

                if (hubAppPool == null)
                {
                    throw new Exception("Hub App Pool not found.");
                }

                var appPool = manager.ApplicationPools.FirstOrDefault((p) => p.Name.Equals(appPoolName));

                if (appPool != null)
                {
                    manager.ApplicationPools.Remove(appPool);
                }

                var newAppPool = manager.ApplicationPools.Add(appPoolName);
                newAppPool.ProcessModel.IdentityType = hubAppPool.ProcessModel.IdentityType;
                newAppPool.Failure.RapidFailProtection = hubAppPool.Failure.RapidFailProtection;
                newAppPool.Recycling.PeriodicRestart.Time = new TimeSpan(hubAppPool.Recycling.PeriodicRestart.Time.Ticks);

                manager.CommitChanges();
            }
        }

        private static void CloneScaleUnitWebRoot()
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd =
                $@"robocopy {Config.HubSiteRoot} {Config.ScaleUnitSiteRoot} /MIR /MT; ";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd, new List<int>() { 0, 1 });
        }
    }
}
