using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using ScaleUnitManagement.Utilities;
using CLI.Actions;

namespace CLI.Menus.WorkloadMovementOptions
{
    internal class PerformEmergencyTransitionToHub : DevToolMenu
    {
        private List<ScaleUnitInstance> sortedNonHubScaleUnits;

        public override async Task Show(int input, string selectionHistory)
        {
            sortedNonHubScaleUnits = Config.NonHubScaleUnitInstances();
            sortedNonHubScaleUnits.Sort();
            List<CLIOption> options = SelectScaleUnitOptions(sortedNonHubScaleUnits, PerformTransition);

            var screen = new SingleSelectScreen(options, selectionHistory, "\nSelect the scale unit where you want to perform emergency transition to hub:\n", "\nScale unit: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task PerformTransition(int input, string selectionHistory)
        {
            string scaleUnitId = sortedNonHubScaleUnits[input - 1].ScaleUnitId;
            var action = new PerformEmergencyTransitionToHubAction(scaleUnitId);
            await action.Execute();
        }
    }
}
