using System.Threading.Tasks;
using CLIFramework;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using System.Collections.Generic;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.SetupToolsOptions
{
    internal class TransitionToHub : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> sortedNonHubScaleUnits = Config.NonHubScaleUnitInstances();
            sortedNonHubScaleUnits.Sort();
            List<CLIOption> options = SelectScaleUnitOptions(sortedNonHubScaleUnits, RunTransition);

            var screen = new SingleSelectScreen(options, selectionHistory, "\nSelect the scale unit where you want to perform emergency transition to hub:\n", "\nScale unit: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task RunTransition(int input, string selectionHistory)
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
