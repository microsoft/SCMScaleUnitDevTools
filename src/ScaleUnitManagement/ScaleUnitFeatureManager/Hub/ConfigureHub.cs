using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public class ConfigureHub : IHubStep
    {
        public string Label()
        {
            return "Configure Hub";
        }

        public float Priority()
        {
            return 2F;
        }


        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            using (var webConfig = new WebConfig())
            {
                SharedWebConfig.Configure(webConfig);

                if (scaleUnit.EnvironmentType == EnvironmentType.VHD || Config.UseSingleOneBox())
                {
                    webConfig.UpdateXElement("Infrastructure.StartStorageEmulator", "false");

                    webConfig.AddValidAudiences();
                }
            }

            if (Config.UseSingleOneBox())
            {
                // Update hosts file
                using (var hosts = new Hosts())
                {
                    hosts.AddMapping(scaleUnit.IpAddress, scaleUnit.DomainSafe());
                }

                // Update IIS binding
                IISAdministrationHelper.CreateSite(
                    siteName: scaleUnit.SiteName(),
                    siteRoot: scaleUnit.SiteRoot(),
                    bindingInformation: scaleUnit.IpAddress + ":443:" + scaleUnit.DomainSafe(),
                    certSubject: scaleUnit.DomainSafe(),
                    appPoolName: scaleUnit.AppPoolName());
            }

            return Task.CompletedTask;
        }
    }
}
