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
    public class ScaleUnitConfigurationManager : AOSCommunicator
    {
        private readonly ScaleUnitEnvironmentConfiguration scaleUnitConfig;

        public ScaleUnitConfigurationManager() : base()
        {
            scaleUnitConfig = new ScaleUnitEnvironmentConfiguration()
            {
                AppId = Config.InterAOSAppId(),
                AppTenant = Config.InterAOSAuthority(),
                HubResourceId = Config.InterAOSAppResourceId(Config.HubScaleUnit()),
                HubUrl = Config.HubScaleUnit().Endpoint(),
                HubS2SEncryptedSecret = Config.InterAOSAppSecret(),
                ScaleUnitType = "1",
            };
        }

        public async Task Configure()
        {
            await ConfigureScaleUnit();
            await WaitForScaleUnitReadiness();

            Console.WriteLine("Done.");
        }

        private async Task ConfigureScaleUnit()
        {
            var aosClient = await GetScaleUnitAosClient();
            ScaleUnitEnvironmentConfiguration configuration = null;
            await ReliableRun.Execute(async () => configuration = await aosClient.WriteScaleUnitConfiguration(scaleUnitConfig), "Writing scale unit configuration");

            configuration.Should().NotBeNull("The AOS should have returned the configuration");
            configuration.WithDeepEqual(scaleUnitConfig).IgnoreSourceProperty((property) => property.HubS2SEncryptedSecret).Assert();
            configuration.HubS2SEncryptedSecret.Should().NotBe(scaleUnitConfig.HubS2SEncryptedSecret, "Secret should have been encrypted by the AOS.");
        }

        private async Task WaitForScaleUnitReadiness()
        {
            var aosClient = await GetScaleUnitAosClient();
            ScaleUnitStatus status = null;
            await ReliableRun.Execute(async () => status = await aosClient.CheckScaleUnitConfigurationStatus(), "Checking scale unit configuration status");

            status.Should().NotBeNull();
            status.Health.Should().Be(ScaleUnitHealthConstants.Running, "Scale unit should be in a healthy/running state.");
        }
    }
}

