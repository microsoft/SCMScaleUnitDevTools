
using System;
using System.Collections.Generic;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagementTests
{
    class ConfigurationHelper
    {

        public CloudAndEdgeConfiguration ConstructTestConfiguration()
        {
            ScaleUnitInstance spoke = new ScaleUnitInstance()
            {
                ScaleUnitId = "@A"
            };

            ScaleUnitInstance hub = new ScaleUnitInstance()
            {
                ScaleUnitId = "@@"
            };

            List<ScaleUnitInstance> scaleUnitInstances = new List<ScaleUnitInstance>();
            scaleUnitInstances.Add(spoke);
            scaleUnitInstances.Add(hub);


            List<ConfiguredDynamicConstraintValue> constraintValues = new List<ConfiguredDynamicConstraintValue>();
            constraintValues.Add(new ConfiguredDynamicConstraintValue()
            {
                Value = "1",
                DomainName = "LegalEntity"
            });

            ConfiguredWorkload workload = new ConfiguredWorkload()
            {
                WorkloadInstanceId = Guid.NewGuid().ToString(),
                Name = "testWorkload",
                ScaleUnitId = "@A",
                ConfiguredDynamicConstraintValues = constraintValues
            };

            List<ConfiguredWorkload> workloads = new List<ConfiguredWorkload>();
            workloads.Add(workload);

            return new CloudAndEdgeConfiguration()
            {
                ScaleUnitConfiguration = scaleUnitInstances,
                Workloads = workloads
            };
        }
    }
}
