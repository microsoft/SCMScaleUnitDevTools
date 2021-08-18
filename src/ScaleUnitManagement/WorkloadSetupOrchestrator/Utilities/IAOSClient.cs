using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CloudAndEdgeLibs.AOS;
using CloudAndEdgeLibs.Contracts;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    public interface IAOSClient
    {
        Task<ScaleUnitEnvironmentConfiguration> WriteScaleUnitConfiguration(ScaleUnitEnvironmentConfiguration config);

        Task<ScaleUnitStatus> CheckScaleUnitConfigurationStatus();

        Task<List<WorkloadInstance>> WriteWorkloadInstances(List<WorkloadInstance> workloadInstances);

        Task<List<WorkloadInstance>> DeleteWorkloadInstances(List<WorkloadInstance> workloadInstances);

        Task<List<Workload>> GetWorkloads();

        Task<List<WorkloadInstance>> GetWorkloadInstances();

        Task<WorkloadInstanceStatus> CheckWorkloadStatus(string workloadInstanceId);

        Task<string> GetWorkloadMovementState(string workloadInstanceId, DateTime afterDateTime);
    }
}
