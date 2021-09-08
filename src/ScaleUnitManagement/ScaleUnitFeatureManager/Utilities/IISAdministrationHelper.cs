using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Web.Administration;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    internal static class IISAdministrationHelper
    {
        internal static void CreateSite(string siteName, string siteRoot,
            string bindingInformation, string certSubject, string appPoolName)
        {
            if (string.IsNullOrEmpty(siteName)
                || string.IsNullOrEmpty(siteRoot)
                || string.IsNullOrEmpty(bindingInformation)
                || string.IsNullOrEmpty(certSubject)
                || string.IsNullOrEmpty(appPoolName))
                throw new ArgumentNullException();

            var scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            if (!scaleUnit.IsHub() && Config.UseSingleOneBox())
                CreateAppPoolForScaleUnit(scaleUnit, appPoolName);

            using (var manager = new ServerManager())
            {
                var site = manager.Sites.FirstOrDefault((s) => s.Name.Equals(siteName));

                if (site != null)
                {
                    manager.Sites.Remove(site);
                }

                site = manager.Sites.Add(siteName, siteRoot, 443);
                site.Applications[0].ApplicationPoolName = appPoolName;

                site.Bindings.Clear();
                site.Bindings.Add(bindingInformation,
                    CertificateStoreHelper.GetCertificateHashFromLocalMachineStore(certSubject),
                    CertificateStoreHelper.PersonalCertificateStoreName);

                manager.CommitChanges();
            }
        }

        internal static void StopAppPoolAndSite(string appPoolName, string siteName)
        {
            using (var manager = new ServerManager())
            {
                var pool = manager.ApplicationPools.FirstOrDefault(p => p.Name.Equals(appPoolName));
                var site = manager.Sites.FirstOrDefault(s => s.Name.Equals(siteName));

                if (pool?.State == ObjectState.Started)
                {
                    Console.WriteLine($"Stopping IIS application pool {pool.Name}..");
                    pool.Stop();
                    Console.WriteLine($"Successfully stopped IIS application pool {pool.Name}");
                }

                if (site?.State == ObjectState.Started)
                {
                    Console.WriteLine($"Stopping IIS site {site.Name}..");
                    site.Stop();
                    Console.WriteLine($"Successfully stopped IIS site {site.Name}");
                }
            }
        }

        internal static void StartAppPoolAndSite(string appPoolName, string siteName)
        {
            using (var manager = new ServerManager())
            {
                var pool = manager.ApplicationPools.FirstOrDefault(p => p.Name.Equals(appPoolName));
                var site = manager.Sites.FirstOrDefault(s => s.Name.Equals(siteName));

                if (pool?.State == ObjectState.Stopped)
                {
                    Console.WriteLine($"Starting IIS application pool {pool.Name}..");
                    pool.Start();
                    Console.WriteLine($"Successfully started IIS application pool {pool.Name}");
                }

                if (site?.State == ObjectState.Stopped)
                {
                    Console.WriteLine($"Starting IIS site {site.Name}..");
                    site.Start();
                    Console.WriteLine($"Successfully started IIS site {site.Name}");
                }
            }
        }

        private static void CreateAppPoolForScaleUnit(ScaleUnitInstance scaleUnit, string appPoolName)
        {
            CloneScaleUnitWebRoot(scaleUnit);

            using (var manager = new ServerManager())
            {
                var hubAppPool = manager.ApplicationPools.FirstOrDefault((p) => p.Name.Equals(Config.HubScaleUnit().AppPoolName()));

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

        private static void CloneScaleUnitWebRoot(ScaleUnitInstance scaleUnit)
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            var robocopyLogPath = Path.Combine(
                Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
                "robocopy.log");

            var cmd =
                $@"robocopy {Config.HubScaleUnit().SiteRoot()} {scaleUnit.SiteRoot()} /MIR /MT /log+:{robocopyLogPath};";

            var ce = new CommandExecutor(cmd);
            ce.RunCommand(0, 1);
        }
    }
}
