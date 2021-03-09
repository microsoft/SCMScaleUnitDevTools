using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public class ConfigureScaleUnit : IScaleUnitStep
    {
        public string Label()
        {
            return "Configure Scale unit";
        }

        public float Priority()
        {
            return 2F;
        }

        public Task Run()
        {
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            if (scaleUnit.EnvironmentType == EnvironmentType.VHD || Config.UseSingleOneBox())
            {
                // Update hosts file
                using (var hosts = new Hosts())
                {
                    hosts.AddMapping(scaleUnit.IpAddress, scaleUnit.DomainSafe());
                    hosts.AddMapping(Config.HubScaleUnit().IpAddress, Config.HubScaleUnit().DomainSafe());
                }

                IISAdministrationHelper.CreateSite(
                    siteName: scaleUnit.SiteName(),
                    siteRoot: scaleUnit.SiteRoot(),
                    bindingInformation: scaleUnit.IpAddress + ":443:" + scaleUnit.DomainSafe(),
                    certSubject: scaleUnit.DomainSafe(),
                    appPoolName: scaleUnit.AppPoolName());
            }

            using (var webConfig = new WebConfig())
            {
                SharedWebConfig.Configure(webConfig);

                if (scaleUnit.EnvironmentType == EnvironmentType.VHD || Config.UseSingleOneBox())
                {
                    webConfig.UpdateXElement("Infrastructure.FullyQualifiedDomainName", scaleUnit.DomainSafe());
                    webConfig.UpdateXElement("Infrastructure.HostName", scaleUnit.DomainSafe());
                    webConfig.UpdateXElement("Infrastructure.HostedServiceName", scaleUnit.ScaleUnitUrlName());

                    string scaleUnitUrl = scaleUnit.Endpoint() + "/";
                    webConfig.UpdateXElement("Infrastructure.HostUrl", scaleUnitUrl);
                    webConfig.UpdateXElement("Infrastructure.SoapServicesUrl", scaleUnitUrl);

                    webConfig.UpdateXElement("DataAccess.Database", scaleUnit.AxDbName);

                    webConfig.AddValidAudiences();
                }
            }

            WifServiceConfig.Update();

            if (Config.UseSingleOneBox())
            {
                CreateScaleUnitBatchService(scaleUnit);
            }

            return Task.CompletedTask;
        }

        private static void CreateScaleUnitBatchService(ScaleUnitInstance scaleUnit)
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            string cmd = $@"
                if (Get-Service '{scaleUnit.BatchServiceName()}' -ErrorAction SilentlyContinue) {{
                    .$env:systemroot\system32\sc.exe delete {scaleUnit.BatchServiceName()};
                }}

                $secpasswd = (new-object System.Security.SecureString);
                $creds = New-Object System.Management.Automation.PSCredential ('NT AUTHORITY\NETWORK SERVICE', $secpasswd);
                New-Service -Name '{scaleUnit.BatchServiceName()}' -BinaryPathName '{scaleUnit.DynamicsBatchExePath()} -service {scaleUnit.WebConfigPath()}' -credential $creds -startupType Manual;
            ";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
