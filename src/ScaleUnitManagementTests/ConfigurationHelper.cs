
using System;
using System.Collections.Generic;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagementTests
{
    internal class ConfigurationHelper
    {

        public CloudAndEdgeConfiguration ConstructTestConfiguration()
        {
            var spoke = new ScaleUnitInstance()
            {
                ScaleUnitId = "@A"
            };

            var hub = new ScaleUnitInstance()
            {
                ScaleUnitId = "@@"
            };

            var scaleUnitInstances = new List<ScaleUnitInstance>
            {
                spoke,
                hub,
            };

            var constraintValues = new List<ConfiguredDynamicConstraintValue>();
            constraintValues.Add(new ConfiguredDynamicConstraintValue()
            {
                Value = "1",
                DomainName = "LegalEntity"
            });

            var workload = new ConfiguredWorkload()
            {
                WorkloadInstanceId = Guid.NewGuid().ToString(),
                Name = "testWorkload",
                ScaleUnitId = "@A",
                ConfiguredDynamicConstraintValues = constraintValues
            };

            var workloads = new List<ConfiguredWorkload> { workload };

            return new CloudAndEdgeConfiguration()
            {
                ScaleUnitConfiguration = scaleUnitInstances,
                Workloads = workloads
            };
        }
    }
}
