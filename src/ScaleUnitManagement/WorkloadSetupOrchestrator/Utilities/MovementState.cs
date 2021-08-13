using System;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    internal class MovementState
    {
        private readonly string state;
        private const string workloadMovementCompletedState = "ScaleUnitWorkloadMovementCompletedState";

        public MovementState(string state)
        {
            this.state = state;
        }

        public string GetStatus()
        {
            if (String.IsNullOrEmpty(state))
            {
                return "not started";
            }
            else if (state.Equals(workloadMovementCompletedState))
            {
                return "finished";
            }
            else
            {
                return "in progress";
            }
        }
    }
}
