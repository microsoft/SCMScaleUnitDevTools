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
        private readonly AOSClient client;

        public WorkloadInstanceManager(AOSClient client)
        {
            this.client = client;
        }

        public async Task<List<WorkloadInstance>> CreateWorkloadInstances()
        {
            List<Workload> workloads = await client.GetWorkloads();
            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>();

            if (!ValidateClientWorkloadsFromConfig(workloads))
            {
                throw new Exception("UserConfig file is missing some of the workload types found on client.");
            }

            foreach (Workload workload in workloads)
            {
                if(workload.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase))
                {
                    workloadInstances.Add(GetSYSWorkloadInstance(workload));
                    continue;
                }

                DynamicConstraintValueFromConfig[] dynamicConstraintValueListFromConfig;

                foreach (WorkloadFromConfig workloadFromConfig in Config.WorkloadList())
                {
                    if (workloadFromConfig.Type.Equals(workload.Name))
                    {
                        dynamicConstraintValueListFromConfig = workloadFromConfig.DynamicConstraintValues.DynamicConstraintValueList;

                        if (!ValidateDynamicConstraints(workloadFromConfig.Type, dynamicConstraintValueListFromConfig))
                        {
                            throw new Exception("Expected domainNames of dynamic constraints for " + workload.Name + " don't match with what is in UserConfig file.");
                        }

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
                            DynamicConstraintValues = GetDynamicConstraintsFromConfig(dynamicConstraintValueListFromConfig),
                        };

                        workloadInstances.Add(workloadInstance);
                    }
                }
            }

            return workloadInstances.OrderBy(wli => wli.VersionedWorkload.Workload.Name, new SysFirstComparer()).ToList();
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

        private bool ValidateClientWorkloadsFromConfig(List<Workload> workloads)
        {
            foreach (Workload workload in workloads)
            {
                if (workload.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase)) continue;

                var isFound = false;

                foreach (WorkloadFromConfig workloadFromConfig in Config.WorkloadList())
                {
                    if (workloadFromConfig.Type.Equals(workload.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isFound = true;
                        break;
                    }
                }

                Console.WriteLine(workload.Name + " workload type is missing from the UserConfig file.");

                if (!isFound) return false;
            }

            return true;
        }

        private bool ValidateDynamicConstraints(string worklaodType, DynamicConstraintValueFromConfig[] dynamicConstraintValueListFromConfig)
        {
            List<string> mesDynamicConstraintDomainNames = new List<string>()
            {
                "LegalEntity", "Site"
            };

            List<string> wesDynamicConstraintDomainNames = new List<string>()
            {
                "LegalEntity", "Site", "Warehouse"
            };

            switch (worklaodType.ToUpper())
            {
                case "MES":
                    if (dynamicConstraintValueListFromConfig.Count() == mesDynamicConstraintDomainNames.Count())
                    {
                        foreach (var dynamicConstraintValue in dynamicConstraintValueListFromConfig)
                        {
                            if (!mesDynamicConstraintDomainNames.Contains(dynamicConstraintValue.DomainName))
                            {
                                Console.WriteLine(dynamicConstraintValue.DomainName
                                    + "is not a valid dynamic constraint for workload type " + worklaodType);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Number of DynamicConstraintValues are not right for workload type " + worklaodType);
                    }
                    break;

                case "WES":
                    if (dynamicConstraintValueListFromConfig.Count() == wesDynamicConstraintDomainNames.Count())
                    {
                        foreach (var dynamicConstraintValue in dynamicConstraintValueListFromConfig)
                        {
                            if (!wesDynamicConstraintDomainNames.Contains(dynamicConstraintValue.DomainName))
                            {
                                Console.WriteLine(dynamicConstraintValue.DomainName
                                    + "is not a valid dynamic constraint for workload type " + worklaodType);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Number of DynamicConstraintValues are not right for workload type " + worklaodType);
                    }
                    break;

                default:
                    Console.WriteLine("Dynamic constraints' domain names of Workload type "
                        + worklaodType + " are not verifed, but the configuration may still succeed.");
                    return false;
            }

            return true;
        }

        private List<DynamicConstraintValue> GetDynamicConstraintsFromConfig(DynamicConstraintValueFromConfig[] dynamicConstraintValueListFromConfig)
        {
            List<DynamicConstraintValue> dynamicConstraintValues = new List<DynamicConstraintValue>();
            foreach (var dynamicConstraintValueFromConfig in dynamicConstraintValueListFromConfig)
            {
                DynamicConstraintValue dynamicConstraintValue = new DynamicConstraintValue()
                {
                    DomainName = dynamicConstraintValueFromConfig.DomainName,
                    Value = dynamicConstraintValueFromConfig.Value,
                };

                dynamicConstraintValues.Add(dynamicConstraintValue);
            }

            return dynamicConstraintValues;
        }

        private GetSYSWorkloadInstance(Workload workload)
        {
            List<DynamicConstraintValue> sysDynamicConstraintValues = new List<DynamicConstraintValue>();
            DynamicConstraintValue dynamicConstraintValue = new DynamicConstraintValue()
            {
                DomainName = "LegalEntity",
                Value = "USMF",
            };

            sysDynamicConstraintValues.Add(dynamicConstraintValue);

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
                DynamicConstraintValues = sysDynamicConstraintValues,
            };

            return workloadInstance;
        }
    }
}
