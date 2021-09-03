using System;
using System.Collections.Generic;
using CloudAndEdgeLibs.Contracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities;

namespace ScaleUnitManagementTests
{
    [TestClass]
    public abstract class DevToolsUnitTest
    {
        protected Mock<IAOSClient> aosClient;
        protected readonly string scaleUnitId = "@A";
        protected readonly string hubId = "@@";
        protected WorkloadInstance exampleWorkload;
        protected List<WorkloadInstance> workloadInstances;

        [TestInitialize]
        public void Initialize()
        {

            ConfigurationHelper configurationHelper = new ConfigurationHelper();

            Func<CloudAndEdgeConfiguration> loadConfigMock = () =>
            {
                return configurationHelper.GetTestConfiguration();
            };

            Config.GetUserConfigImplementation = loadConfigMock;

            exampleWorkload = configurationHelper.GetExampleWorkload();
            workloadInstances = new List<WorkloadInstance> { exampleWorkload };

            aosClient = new Mock<IAOSClient>();

            aosClient.Setup(x => x.GetWorkloadInstances())
                .ReturnsAsync(new List<WorkloadInstance>(workloadInstances));

        }
    }
}
