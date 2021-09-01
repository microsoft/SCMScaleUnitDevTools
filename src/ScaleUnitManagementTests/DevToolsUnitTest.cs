using System;
using System.Collections.Generic;
using System.Text;
using CloudAndEdgeLibs.Contracts;
using Moq;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagementTests
{
    public abstract class DevToolsUnitTest
    {
        protected Mock<IAOSClient> aosClient;
        protected readonly string scaleUnitId = "@A";
        protected readonly string hubId = "@@";
        protected WorkloadInstance exampleWorkload;

        public void Initialize()
        {
            aosClient = new Mock<IAOSClient>();

            ConfigurationHelper configurationHelper = new ConfigurationHelper();

            Func<CloudAndEdgeConfiguration> loadConfigMock = () =>
            {
                return configurationHelper.GetTestConfiguration();
            };

            Config.GetUserConfigImplementation = loadConfigMock;

            exampleWorkload = configurationHelper.GetExampleWorkload();
        }
    }
}
