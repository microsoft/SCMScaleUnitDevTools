using System;
using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.Actions
{
    internal class PerformEmergencyTransitionToHubAction : ContextualAction
    {
        public PerformEmergencyTransitionToHubAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            try
            {
                var workloadMover = new WorkloadMover();
                await workloadMover.EmergencyTransitionToHub();
                Console.WriteLine("Done");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to run emergency transition to hub:\n{ex}");
            }
        }
    }
}
