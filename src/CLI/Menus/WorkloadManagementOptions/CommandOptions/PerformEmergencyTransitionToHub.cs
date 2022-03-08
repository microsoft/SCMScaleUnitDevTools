using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using ScaleUnitManagement.Utilities;
using CLI.Actions;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class PerformEmergencyTransitionToHub : ActionMenu
    {
        private List<ScaleUnitInstance> sortedNonHubScaleUnits;

        public override string Label => "Perform emergency transition from scale unit to hub";

        protected override IAction Action => new PerformEmergencyTransitionToHubAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            sortedNonHubScaleUnits = Config.NonHubScaleUnitInstances();
            sortedNonHubScaleUnits.Sort();
            List<CLIOption> options = SelectScaleUnitOptions(sortedNonHubScaleUnits, TransitionToHub);

            var screen = new SingleSelectScreen(options, selectionHistory, "\nSelect the scale unit where you want to perform emergency transition to hub:\n", "\nScale unit: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task TransitionToHub(int input, string selectionHistory)
        {
            scaleUnitId = sortedNonHubScaleUnits[input - 1].ScaleUnitId;
            await Action.Execute();
        }
    }
}
