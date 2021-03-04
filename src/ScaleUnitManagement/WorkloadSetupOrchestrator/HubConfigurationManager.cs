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
    public class HubConfigurationManager
    {
        private AOSClient hubAosClient = null;
        private readonly ScaleUnitEnvironmentConfiguration hubConfig;
        private readonly ScaleUnitInstance scaleUnit;

        public HubConfigurationManager()
        {
            scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            hubConfig = new ScaleUnitEnvironmentConfiguration()
            {
                AppId = Config.InterAOSAppId(),
                AppTenant = Config.InterAOSAuthority(),
                HubResourceId = scaleUnit.ResourceId(),
                HubUrl = scaleUnit.Endpoint(),
                HubS2SEncryptedSecret = Config.InterAOSAppSecret(),
                ScaleUnitType = "0",
            };
        }

        private async Task EnsureClientInitialized()
        {
            if (hubAosClient is null)
            {
                hubAosClient = await AOSClient.Construct(scaleUnit);
            }
        }

        public async Task Configure()
        {
            await ConfigureHub();
            await WaitForHubReadiness();

            Console.WriteLine("Done.");
        }

        private async Task ConfigureHub()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(
                async () =>
                {
                    ScaleUnitEnvironmentConfiguration configuration = await hubAosClient.WriteScaleUnitConfiguration(hubConfig);

                    configuration.Should().NotBeNull("The AOS should have returned the configuration");
                    configuration.WithDeepEqual(hubConfig).IgnoreSourceProperty((property) => property.HubS2SEncryptedSecret).Assert();
                    configuration.HubS2SEncryptedSecret.Should().NotBe(hubConfig.HubS2SEncryptedSecret, "Secret should have been encrypted by the AOS.");
                },
                "Hub configuration");
        }

        private async Task WaitForHubReadiness()
        {
            await EnsureClientInitialized();

            await ReliableRun.Execute(
                async () =>
                {
                    ScaleUnitStatus status = await hubAosClient.CheckScaleUnitConfigurationStatus();

                    status.Should().NotBeNull();
                    status.Health.Should().Be(ScaleUnitHealthConstants.Running, "Hub should be in a healthy/running state.");

                },
                "Wait for hub readiness");
        }
    }
}
