using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    class WorkloadInstanceManager
    {
        private readonly IAOSClient client;
        private static readonly string ReadyState = "Running";
        private static readonly string InstallingState = "Installing";
        private static readonly string StoppedState = "Stopped";

        public WorkloadInstanceManager(IAOSClient client)
        {
            this.client = client;
        }

        public async Task<List<WorkloadInstance>> CreateWorkloadInstances()
        {
            if (Config.WorkloadList() == null || Config.WorkloadList().Any() == false)
            {
                throw new Exception("No workload is defined in the UserConfig file.");
            }

            List<Workload> workloads = await client.GetWorkloads();
            List<WorkloadInstance> workloadInstances = new List<WorkloadInstance>();
            List<WorkloadInstance> sysWorkloadInstances = new List<WorkloadInstance>();

            if (!ValidateIfConfigWorkloadsExistOnClient(workloads))
            {
                throw new Exception("UserConfig file has some workload types which are not found on client.");
            }

            foreach (Workload workload in workloads)
            {
                if (workload.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase))
                {
                    sysWorkloadInstances = GetSYSWorkloadInstancesPerLegalEntity(workload);
                    continue;
                }

                List<ConfiguredDynamicConstraintValue> configuredDynamicConstraintValues;

                foreach (ConfiguredWorkload configuredworkload in Config.WorkloadList())
                {
                    if (configuredworkload.Name.Equals(workload.Name))
                    {
                        configuredDynamicConstraintValues = configuredworkload.ConfiguredDynamicConstraintValues;

                        if (!ValidateDynamicConstraints(configuredworkload.Name, configuredDynamicConstraintValues))
                        {
                            throw new Exception("Expected domainNames of dynamic constraints for " + workload.Name + " don't match with what is in UserConfig file.");
                        }

                        ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(configuredworkload.ScaleUnitId);

                        WorkloadInstance workloadInstance = new WorkloadInstance()
                        {
                            Id = configuredworkload.WorkloadInstanceId,
                            LogicalEnvironmentId = Config.LogicalEnvironmentId,
                            ExecutingEnvironment = new List<TemporalAssignment>()
                            {
                                new TemporalAssignment()
                                {
                                    EffectiveDate = GetWorkloadEffectiveDate(),
                                    Environment = new PhysicalEnvironmentReference()
                                    {
                                        Id = Config.ScaleUnitEnvironmentId,
                                        Name = scaleUnit.ScaleUnitName,
                                        ScaleUnitId = scaleUnit.ScaleUnitId,
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
                            DynamicConstraintValues = GetConfiguredDynamicConstraintValues(configuredDynamicConstraintValues),
                        };

                        workloadInstances.Add(workloadInstance);
                    }
                }
            }

            WorkloadInstanceTopologicalSortUtil topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(sysWorkloadInstances.Concat(workloadInstances).ToList());
            return topologicalSortUtil.Sort();
        }

        private bool ValidateIfConfigWorkloadsExistOnClient(List<Workload> workloads)
        {
            List<string> nonValidWorkloadNames = new List<string>();

            foreach (ConfiguredWorkload configuredworkload in Config.WorkloadList())
            {
                if (configuredworkload.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase)) continue;

                var isFound = false;

                foreach (Workload workload in workloads)
                {
                    if (configuredworkload.Name.Equals(workload.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        isFound = true;
                        break;
                    }
                }

                if (!isFound)
                {
                    nonValidWorkloadNames.Add(configuredworkload.Name);
                }
            }

            if (nonValidWorkloadNames != null && nonValidWorkloadNames.Any())
            {
                Console.WriteLine("The following workload names were not found on client. Please remove them from the UserConfig file and try again.");
                nonValidWorkloadNames.ForEach(Console.WriteLine);
                return false;
            }

            return true;
        }

        private bool ValidateDynamicConstraints(string workloadName, List<ConfiguredDynamicConstraintValue> configuredDynamicConstraintValues)
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
                    List<string> nonValidDomainNamesMESInConfig = (List<string>)configuredDynamicConstraintValues.Select(x => x.DomainName)
                        .Except(mesDynamicConstraintDomainNames).ToList();
                    List<string> missingDomainNamesMESInConfig = (List<string>)mesDynamicConstraintDomainNames
                        .Except(configuredDynamicConstraintValues.Select(x => x.DomainName)).ToList();

                    if (missingDomainNamesMESInConfig != null && missingDomainNamesMESInConfig.Any())
                    {
                        Console.WriteLine("Missing domain names in the UserConfig file for workload name " + workloadName);
                        missingDomainNamesMESInConfig.ForEach(Console.WriteLine);
                    }

                    if (nonValidDomainNamesMESInConfig != null && nonValidDomainNamesMESInConfig.Any())
                    {
                        Console.WriteLine("Invalid domain names in the UserConfig file for workload name " + workloadName);
                        nonValidDomainNamesMESInConfig.ForEach(Console.WriteLine);
                    }

                    if ((missingDomainNamesMESInConfig != null && missingDomainNamesMESInConfig.Any())
                        || (nonValidDomainNamesMESInConfig != null && nonValidDomainNamesMESInConfig.Any()))
                    {
                        return false;
                    }

                    break;

                case "WES":
                    List<string> nonValidDomainNamesWESInConfig = (List<string>)configuredDynamicConstraintValues.Select(x => x.DomainName).Except(wesDynamicConstraintDomainNames).ToList();
                    List<string> missingDomainNamesWESInConfig = (List<string>)wesDynamicConstraintDomainNames.Except(configuredDynamicConstraintValues.Select(x => x.DomainName)).ToList();

                    if (missingDomainNamesWESInConfig != null && missingDomainNamesWESInConfig.Any())
                    {
                        Console.WriteLine("Missing domain names in the UserConfig file for workload name " + workloadName);
                        missingDomainNamesWESInConfig.ForEach(Console.WriteLine);
                    }

                    if (nonValidDomainNamesWESInConfig != null && nonValidDomainNamesWESInConfig.Any())
                    {
                        Console.WriteLine("Invalid domain names in the UserConfig file for workload name " + workloadName);
                        nonValidDomainNamesWESInConfig.ForEach(Console.WriteLine);
                    }

                    if ((missingDomainNamesWESInConfig != null && missingDomainNamesWESInConfig.Any())
                        || (nonValidDomainNamesWESInConfig != null && nonValidDomainNamesWESInConfig.Any()))
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

        private List<DynamicConstraintValue> GetConfiguredDynamicConstraintValues(List<ConfiguredDynamicConstraintValue> configuredDynamicConstraintValues)
        {
            List<DynamicConstraintValue> dynamicConstraintValues = new List<DynamicConstraintValue>();
            foreach (ConfiguredDynamicConstraintValue configuredDynamicConstraintValue in configuredDynamicConstraintValues)
            {
                DynamicConstraintValue dynamicConstraintValue = new DynamicConstraintValue()
                {
                    DomainName = configuredDynamicConstraintValue.DomainName,
                    Value = configuredDynamicConstraintValue.Value,
                };

                dynamicConstraintValues.Add(dynamicConstraintValue);
            }

            return dynamicConstraintValues;
        }

        private List<WorkloadInstance> GetSYSWorkloadInstancesPerLegalEntity(Workload workload)
        {
            List<string> uniqueLegalEntityValues = Config.UniqueLegalEntityValues();
            List<WorkloadInstance> sysWorkloadInstances = new List<WorkloadInstance>();
            List<string> sysWorkloadInstanceIds = Config.SYSWorkloadInstanceIds();
            int count = 0;

            foreach (string legalEntityValue in uniqueLegalEntityValues)
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
                    Id = sysWorkloadInstanceIds[count++],
                    LogicalEnvironmentId = Config.LogicalEnvironmentId,
                    ExecutingEnvironment = new List<TemporalAssignment>()
                    {
                        new TemporalAssignment()
                        {
                            EffectiveDate = GetWorkloadEffectiveDate(),
                            Environment = new PhysicalEnvironmentReference()
                            {
                                Id = Config.ScaleUnitEnvironmentId,
                                Name = Config.NonHubScaleUnit().ScaleUnitName,
                                ScaleUnitId = Config.NonHubScaleUnit().ScaleUnitId
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

        private static DateTime GetWorkloadEffectiveDate()
        {
            var effectiveDate = DateTime.UtcNow;
            // Always adding 5 minutes to the effective date to mitigate Bug 616219 on the AX.
            effectiveDate.AddMinutes(5);
            return effectiveDate;
        }

        public static async Task<WorkloadInstanceStatus> GetWorkloadInstanceStatus(IAOSClient client, string workloadInstanceId)
        {
            return await client.CheckWorkloadStatus(workloadInstanceId);
        }

        public static async Task<bool> IsWorkloadInstanceInReadyState(IAOSClient client, WorkloadInstance workloadInstance)
        {
            WorkloadInstanceStatus status = await GetWorkloadInstanceStatus(client, workloadInstance.Id);
            return status.Health == ReadyState;
        }

        public static async Task<bool> IsWorkloadInstanceInInstallingState(IAOSClient client, WorkloadInstance workloadInstance)
        {
            WorkloadInstanceStatus status = await GetWorkloadInstanceStatus(client, workloadInstance.Id);
            return status.Health == InstallingState;
        }

        public static async Task<bool> IsWorkloadInStoppedState(IAOSClient client, WorkloadInstance workloadInstance)
        {
            WorkloadInstanceStatus status = await GetWorkloadInstanceStatus(client, workloadInstance.Id);
            return status.Health == StoppedState;
        }

        public static bool IsWorkloadSYSOnSpoke(WorkloadInstance workloadInstance)
        {
            var scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            var name = workloadInstance.VersionedWorkload.Workload.Name;
            return name.Equals("SYS") && !scaleUnit.IsHub();
        }

    }
}
