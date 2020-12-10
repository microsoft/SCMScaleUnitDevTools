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
            if (Config.WorkloadList()?.Any() == false)
            {
                throw new Exception("No workload is defined in the UserConfig file.");
            }

            List<Workload> workloads = await client.GetWorkloads();
            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>();

            if (!ValidateIfConfigWorkloadsExistOnClient(workloads))
            {
                throw new Exception("UserConfig file has some workload types which are not found on client.");
            }

            foreach (Workload workload in workloads)
            {
                if (workload.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase))
                {
                    workloadInstances.Concat(GetSYSWorkloadInstancesPerLegalEntity(workload));
                    continue;
                }

                List<DynamicConstraintValueFromConfig> dynamicConstraintValueListFromConfig;

                foreach (WorkloadFromConfig workloadFromConfig in Config.WorkloadList())
                {
                    if (workloadFromConfig.Name.Equals(workload.Name))
                    {
                        dynamicConstraintValueListFromConfig = workloadFromConfig.DynamicConstraintValuesFromConfig;

                        if (!ValidateDynamicConstraints(workloadFromConfig.Name, dynamicConstraintValueListFromConfig))
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

        private bool ValidateIfConfigWorkloadsExistOnClient(List<Workload> workloads)
        {
            List<string> nonValidWorkloadNames = new List<string>();

            foreach (WorkloadFromConfig workloadFromConfig in Config.WorkloadList())
            {
                if (workloadFromConfig.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase)) continue;

                var isFound = false;

                foreach (Workload workload in workloads)
                {
                    if (workloadFromConfig.Name.Equals(workload.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    nonValidWorkloadNames.Add(workloadFromConfig.Name);
                }
            }

            if (nonValidWorkloadNames?.Any() == false)
            {
                Console.WriteLine("The following workload names were not found on client. Please remove them from the UserConfig file and try again.");
                nonValidWorkloadNames.ForEach(Console.WriteLine);
                return false;
            }

            return true;
        }

        private bool ValidateDynamicConstraints(string workloadName, List<DynamicConstraintValueFromConfig> dynamicConstraintValueListFromConfig)
        {
            List<string> mesDynamicConstraintDomainNames = new List<string>()
            {
                "LegalEntity", "Site"
            };

            List<string> wesDynamicConstraintDomainNames = new List<string>()
            {
                "LegalEntity", "Site", "Warehouse"
            };

            switch (workloadName.ToUpper())
            {
                case "MES":
                    List<string> nonValidDomainNamesMESInConfig = (List<string>)dynamicConstraintValueListFromConfig.Select(x => x.DomainName)
                        .Except(mesDynamicConstraintDomainNames);
                    List<string> missingDomainNamesMESInConfig = (List<string>)mesDynamicConstraintDomainNames
                        .Except(dynamicConstraintValueListFromConfig.Select(x => x.DomainName));

                    if (missingDomainNamesMESInConfig?.Any() == false)
                    {
                        Console.WriteLine("Missing domain names in the UserConfig file for workload name " + workloadName);
                        missingDomainNamesMESInConfig.ForEach(Console.WriteLine);
                    }

                    if (nonValidDomainNamesMESInConfig?.Any() == false)
                    {
                        Console.WriteLine("Invalid domain names in the UserConfig file for workload name " + workloadName);
                        nonValidDomainNamesMESInConfig.ForEach(Console.WriteLine);
                    }

                    if (missingDomainNamesMESInConfig?.Any() == false || nonValidDomainNamesMESInConfig?.Any() == false)
                    {
                        return false;
                    }

                    break;

                case "WES":
                    List<string> nonValidDomainNamesWESInConfig = (List<string>)dynamicConstraintValueListFromConfig.Select(x => x.DomainName).Except(wesDynamicConstraintDomainNames);
                    List<string> missingDomainNamesWESInConfig = (List<string>)wesDynamicConstraintDomainNames.Except(dynamicConstraintValueListFromConfig.Select(x => x.DomainName));

                    if (missingDomainNamesWESInConfig?.Any() == false)
                    {
                        Console.WriteLine("Missing domain names in the UserConfig file for workload name " + workloadName);
                        missingDomainNamesWESInConfig.ForEach(Console.WriteLine);
                    }

                    if (nonValidDomainNamesWESInConfig?.Any() == false)
                    {
                        Console.WriteLine("Invalid domain names in the UserConfig file for workload name " + workloadName);
                        nonValidDomainNamesWESInConfig.ForEach(Console.WriteLine);
                    }

                    if (missingDomainNamesWESInConfig?.Any() == false || nonValidDomainNamesWESInConfig?.Any() == false)
                    {
                        return false;
                    }

                    break;

                default:
                    Console.WriteLine("Dynamic constraints' domain names of Workload name "
                        + workloadName + " are not verifed, but the configuration may still succeed.");
                    return true;
            }

            return true;
        }

        private List<DynamicConstraintValue> GetDynamicConstraintsFromConfig(List<DynamicConstraintValueFromConfig> dynamicConstraintValueListFromConfig)
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

        private List<WorkloadInstance> GetSYSWorkloadInstancesPerLegalEntity(Workload workload)
        {
            List<string> uniqueLegalEntityValues = Config.UniqueLegalEntityValues();
            List<WorkloadInstance> sysWorkloadInstances = new List<WorkloadInstance>();

            foreach(string legalEntityValue in uniqueLegalEntityValues)
            {
                List<DynamicConstraintValue> sysDynamicConstraintValues = new List<DynamicConstraintValue>();
                DynamicConstraintValue dynamicConstraintValue = new DynamicConstraintValue()
                {
                    DomainName = "LegalEntity",
                    Value = legalEntityValue,
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

                sysWorkloadInstances.Add(workloadInstance);
            }

            return sysWorkloadInstances;
        }
    }
}
