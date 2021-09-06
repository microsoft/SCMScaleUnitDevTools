using System;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;
using DeepEqual.Syntax;
using FluentAssertions;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator
{
    public class HubConfigurationManager : AOSCommunicator
    {
        private readonly ScaleUnitEnvironmentConfiguration hubConfig;

        public HubConfigurationManager() : base()
        {
            hubConfig = new ScaleUnitEnvironmentConfiguration()
            {
                AppId = Config.InterAOSAppId(),
                AppTenant = Config.InterAOSAuthority(),
                HubResourceId = Config.InterAOSAppResourceId(scaleUnit),
                HubUrl = scaleUnit.Endpoint(),
                HubS2SEncryptedSecret = Config.InterAOSAppSecret(),
                ScaleUnitType = "0",
            };
        }

        public async Task Configure()
        {
            await ConfigureHub();
            await WaitForHubReadiness();

            Console.WriteLine("Done.");
        }

        private async Task ConfigureHub()
        {
            var aosClient = await GetScaleUnitAosClient();
            ScaleUnitEnvironmentConfiguration configuration = null;
            await ReliableRun.Execute(async () => configuration = await aosClient.WriteScaleUnitConfiguration(hubConfig), "Writing scale unit configuration");

            configuration.Should().NotBeNull("The AOS should have returned the configuration");
            configuration.WithDeepEqual(hubConfig).IgnoreSourceProperty((property) => property.HubS2SEncryptedSecret).Assert();
            configuration.HubS2SEncryptedSecret.Should().NotBe(hubConfig.HubS2SEncryptedSecret, "Secret should have been encrypted by the AOS.");
        }

        private async Task WaitForHubReadiness()
        {
            var aosClient = await GetScaleUnitAosClient();
            ScaleUnitStatus status = null;
            await ReliableRun.Execute(async () => status = await aosClient.CheckScaleUnitConfigurationStatus(), "Checking scale unit configuration status");
            status.Should().NotBeNull();
            status.Health.Should().Be(ScaleUnitHealthConstants.Running, "Hub should be in a healthy/running state.");
        }
    }
}
