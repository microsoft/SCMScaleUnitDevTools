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
    public class ScaleUnitConfigurationManager
    {
        private AOSClient scaleUnitAosClient = null;
        private readonly ScaleUnitEnvironmentConfiguration scaleUnitConfig;
        private readonly ScaleUnitInstance scaleUnit;

        public ScaleUnitConfigurationManager()
        {
            scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            scaleUnitConfig = new ScaleUnitEnvironmentConfiguration()
            {
                AppId = Config.InterAOSAppId(),
                AppTenant = Config.InterAOSAuthority(),
                HubResourceId = Config.HubScaleUnit().ResourceId(),
                HubUrl = Config.HubScaleUnit().Endpoint(),
                HubS2SEncryptedSecret = Config.InterAOSAppSecret(),
                ScaleUnitType = "1",
            };
        }

        private async Task EnsureClientInitialized()
        {
            if (scaleUnitAosClient is null)
            {
                scaleUnitAosClient = await AOSClient.Construct(scaleUnit);
            }
        }

        public async Task Configure()
        {
            await ConfigureScaleUnit();
            await WaitForScaleUnitReadiness();

            Console.WriteLine("Done.");
        }

        private async Task ConfigureScaleUnit()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                ScaleUnitEnvironmentConfiguration configuration = await scaleUnitAosClient.WriteScaleUnitConfiguration(scaleUnitConfig);

                configuration.Should().NotBeNull("The AOS should have returned the configuration");
                configuration.WithDeepEqual(scaleUnitConfig).IgnoreSourceProperty((property) => property.HubS2SEncryptedSecret).Assert();
                configuration.HubS2SEncryptedSecret.Should().NotBe(scaleUnitConfig.HubS2SEncryptedSecret, "Secret should have been encrypted by the AOS.");
            }, "Scale unit configuration");
        }

        private async Task WaitForScaleUnitReadiness()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(async () =>
            {
                ScaleUnitStatus status = await scaleUnitAosClient.CheckScaleUnitConfigurationStatus();

                status.Should().NotBeNull();
                status.Health.Should().Be(ScaleUnitHealthConstants.Running, "Scale unit should be in a healthy/running state.");
            }, "Wait for scale unit readiness");
        }
    }
}

