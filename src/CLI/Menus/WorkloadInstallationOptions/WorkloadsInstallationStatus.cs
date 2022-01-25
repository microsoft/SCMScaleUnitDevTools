using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadInstallationOptions
{
    internal class WorkloadsInstallationStatus : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), ShowWorkloadInstallationStatusForScaleUnit);

            var screen = new SingleSelectScreen(options, selectionHistory, "Show status of workloads installation on:\n", "\nEnvironment?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task ShowWorkloadInstallationStatusForScaleUnit(int input, string selectionHistory)
        {
            string scaleUnitId = GetSortedScaleUnits()[input - 1].ScaleUnitId;
            var action = new WorkloadsInstallationStatusAction(scaleUnitId);
            await action.Execute();
        }
    }
}
