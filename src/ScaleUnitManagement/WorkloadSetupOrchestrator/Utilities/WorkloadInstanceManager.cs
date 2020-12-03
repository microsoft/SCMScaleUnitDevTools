using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    class WorkloadInstanceManager
    {
        private AOSClient client;

        private readonly List<DynamicConstraintValue> sysDynamicConstraintValues = new List<DynamicConstraintValue>()
        {
            new DynamicConstraintValue()
            {
                DomainName = "LegalEntity",
                Value = "USMF",
            },
        };

        private readonly List<DynamicConstraintValue> mesDynamicConstraintValues = new List<DynamicConstraintValue>()
        {
            new DynamicConstraintValue()
            {
                DomainName = "LegalEntity",
                Value = "USMF",
            },
            new DynamicConstraintValue()
            {
                DomainName = "Site",
                Value = "1",
            },
        };

        private readonly List<DynamicConstraintValue> wesDynamicConstraintValues = new List<DynamicConstraintValue>()
        {
            new DynamicConstraintValue()
            {
                DomainName = "LegalEntity",
                Value = "USMF",
            },
            new DynamicConstraintValue()
            {
                DomainName = "Site",
                Value = "2",
            },
            new DynamicConstraintValue()
            {
                DomainName = "Warehouse",
                Value = "24",
            },
        };

        public WorkloadInstanceManager(AOSClient client)
        {
            this.client = client;
        }

        public async Task<List<WorkloadInstance>> CreateWorkloadInstances()
        {
            List<Workload> workloads = await client.GetWorkloads();
            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>();

            foreach (Workload workload in workloads)
            {
                WorkloadInstance workloadInstance = new WorkloadInstance()
                {
                    Id = GetWorkloadInstanceId(workload.Name),
                    LogicalEnvironmentId = Config.LogicalEnvironmentId,
                    ExecutingEnvironment = new List<TemporalAssignment>()
                    {
                        new TemporalAssignment()
                        {
                            EffectiveDate = DateTime.Now,
                            Environment = new PhysicalEnvironmentReference()
                            {
                                Id = Config.ScaleUnitEnvironmentId,
                                Name = Config.ScaleUnitName(),
                                ScaleUnitId = Config.ScaleUnitId(),
                            },
                        },
                    },
                    VersionedWorkload = new VersionedWorkload()
                    {
                        Id = Guid.NewGuid().ToString(),
                        PlatformPackageId = Guid.NewGuid().ToString(),
                        ApplicationPackageId = Guid.NewGuid().ToString(),
                        CustomizationPackageId = Guid.NewGuid().ToString(),
                        Hash = Config.WorkloadDefinitionHash,
                        LogicalEnvironmentId = Config.LogicalEnvironmentId,
                        Workload = workload,
                    },
                    DynamicConstraintValues = GetDynamicConstrains(workload.Name),
                };

                workloadInstances.Add(workloadInstance);
            }

            return workloadInstances.OrderBy(wli => wli.VersionedWorkload.Workload.Name, new SysFirstComparer()).ToList();
        }

        private List<DynamicConstraintValue> GetDynamicConstrains(string workloadInstanceName)
        {
            switch (workloadInstanceName)
            {
                case "SYS": return this.sysDynamicConstraintValues;
                case "MES": return this.mesDynamicConstraintValues;
                case "WES": return this.wesDynamicConstraintValues;
                default: return new List<DynamicConstraintValue>();
            }
        }

        private string GetWorkloadInstanceId(string workloadInstanceName)
        {
            switch (workloadInstanceName)
            {
                case "SYS": return Config.SysWorkloadInstanceId;
                case "MES": return Config.MesWorkloadInstanceId;
                case "WES": return Config.WesWorkloadInstanceId;
                default: return Guid.NewGuid().ToString();
            }
        }

        private class SysFirstComparer : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                if (x.Equals("SYS", StringComparison.OrdinalIgnoreCase))
                {
                    return -1;
                }
                else if (y.Equals("SYS", StringComparison.OrdinalIgnoreCase))
                {
                    return 1;
                }

                return 0;
            }
        }
    }
}
