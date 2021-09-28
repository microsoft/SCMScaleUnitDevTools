
using System;
using System.Collections.Generic;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagementTests
{
    internal class ConfigurationHelper
    {
        private readonly string scaleUnitId = "@A";
        private readonly string hubId = "@@";
        private readonly string workloadInstanceId;
        private readonly string versionedWorkloadId;
        private readonly string workloadInstanceName;
        private readonly CloudAndEdgeConfiguration configuration;

        public ConfigurationHelper()
        {
            workloadInstanceId = Guid.NewGuid().ToString();
            versionedWorkloadId = Guid.NewGuid().ToString();
            workloadInstanceName = "testWorkload";

            var spoke = new ScaleUnitInstance()
            {
                ScaleUnitId = scaleUnitId
            };

            var hub = new ScaleUnitInstance()
            {
                ScaleUnitId = hubId
            };

            var scaleUnitInstances = new List<ScaleUnitInstance>
            {
                spoke,
                hub,
            };

            var constraintValues = new List<ConfiguredDynamicConstraintValue>
            {
                new ConfiguredDynamicConstraintValue()
                {
                    Value = "1",
                    DomainName = "LegalEntity"
                }
            };

            var workload = new ConfiguredWorkload()
            {
                WorkloadInstanceId = workloadInstanceId,
                Name = workloadInstanceName,
                ScaleUnitId = scaleUnitId,
                ConfiguredDynamicConstraintValues = constraintValues
            };

            var workloads = new List<ConfiguredWorkload> { workload };

            configuration = new CloudAndEdgeConfiguration()
            {
                ScaleUnitConfiguration = scaleUnitInstances,
                Workloads = workloads
            };
        }

        internal CloudAndEdgeConfiguration GetTestConfiguration()
        {
            return configuration;
        }

        internal WorkloadInstance GetExampleWorkload()
        {
            var workloadInstance = new WorkloadInstance
            {
                Id = workloadInstanceId,
                VersionedWorkload = new VersionedWorkload
                {
                    Workload = new Workload { Name = workloadInstanceName },
                    Id = versionedWorkloadId,
                },
            };
            workloadInstance.ExecutingEnvironment.Add(new TemporalAssignment
            {
                EffectiveDate = DateTime.UtcNow,
                Environment = new PhysicalEnvironmentReference() { ScaleUnitId = scaleUnitId },
            });
            return workloadInstance;
        }
    }
}
