using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.QueryOptions
{
    internal class WorkloadsInstallationStatus : ActionMenu
    {
        public override string Label => "Show workloads installation status";

        protected override IAction Action => new WorkloadsInstallationStatusAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);

            var screen = new SingleSelectScreen(options, selectionHistory, "Show status of workloads installation on:\n", "\nEnvironment?: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
