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
        private const string ReadyState = "Running";
        private const string InstallingState = "Installing";
        private const string StoppedState = "Stopped";

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
            var workloadInstances = new List<WorkloadInstance>();
            var sysWorkloadInstances = new List<WorkloadInstance>();

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

                        var workloadInstance = new WorkloadInstance()
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

            var topologicalSortUtil = new WorkloadInstanceTopologicalSortUtil(sysWorkloadInstances.Concat(workloadInstances).ToList());
            return topologicalSortUtil.Sort();
        }

        private bool ValidateIfConfigWorkloadsExistOnClient(List<Workload> workloads)
        {
            var nonValidWorkloadNames = new List<string>();

            foreach (ConfiguredWorkload configuredworkload in Config.WorkloadList())
            {
                if (configuredworkload.Name.Equals("SYS", StringComparison.OrdinalIgnoreCase)) continue;

                bool isFound = false;

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
            var mesDynamicConstraintDomainNames = new List<string>()
            {
                "LegalEntity", "Site"
            };

            var wesDynamicConstraintDomainNames = new List<string>()
            {
                "LegalEntity", "Site", "Warehouse"
            };

            switch (workloadName.ToUpper())
            {
                case "MES":
                    var nonValidDomainNamesMESInConfig = configuredDynamicConstraintValues.Select(x => x.DomainName)
                        .Except(mesDynamicConstraintDomainNames).ToList();
                    var missingDomainNamesMESInConfig = mesDynamicConstraintDomainNames
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
                    var nonValidDomainNamesWESInConfig = configuredDynamicConstraintValues.Select(x => x.DomainName).Except(wesDynamicConstraintDomainNames).ToList();
                    var missingDomainNamesWESInConfig = wesDynamicConstraintDomainNames.Except(configuredDynamicConstraintValues.Select(x => x.DomainName)).ToList();

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
            var dynamicConstraintValues = new List<DynamicConstraintValue>();
            foreach (ConfiguredDynamicConstraintValue configuredDynamicConstraintValue in configuredDynamicConstraintValues)
            {
                var dynamicConstraintValue = new DynamicConstraintValue()
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
            var sysWorkloadInstances = new List<WorkloadInstance>();
            List<string> sysWorkloadInstanceIds = Config.SYSWorkloadInstanceIds();
            int count = 0;

            foreach (string legalEntityValue in uniqueLegalEntityValues)
            {
                var sysDynamicConstraintValues = new List<DynamicConstraintValue>();
                var dynamicConstraintValue = new DynamicConstraintValue()
                {
                    DomainName = "LegalEntity",
                    Value = legalEntityValue,
                };

                sysDynamicConstraintValues.Add(dynamicConstraintValue);

                var workloadInstance = new WorkloadInstance()
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
            // Always adding 5 minutes to the effective date to mitigate Bug 616219 on the AX.
            DateTime effectiveDate = DateTime.UtcNow.AddMinutes(5);
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
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            string name = workloadInstance.VersionedWorkload.Workload.Name;
            return name.Equals("SYS") && !scaleUnit.IsHub();
        }
    }
}
