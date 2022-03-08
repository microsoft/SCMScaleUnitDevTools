using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class InstallWorkloads : ActionMenu
    {
        public override string Label => "Install workloads";

        protected override IAction Action => new InstallWorkloadsAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Install workloads on:\n", "\nEnvironment to install the workloads on?: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
