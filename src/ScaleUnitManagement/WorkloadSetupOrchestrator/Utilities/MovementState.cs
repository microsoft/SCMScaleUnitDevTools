using System;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    class MovementState
    {
        private readonly string state;
        private readonly string workloadMovementCompletedState = "ScaleUnitWorkloadMovementCompletedState";

        public MovementState(string state)
        {
            this.state = state;
        }

        public string getStatus()
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
