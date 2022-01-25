using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadInstallationOptions
{
    internal class InstallWorkloads : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), InstallWorkloadsForScaleUnit);
            var screen = new SingleSelectScreen(options, selectionHistory, "Install workloads on:\n", "\nEnvironment to install the workloads on?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task InstallWorkloadsForScaleUnit(int input, string selectionHistory)
        {
            string scaleUnitId = GetSortedScaleUnits()[input - 1].ScaleUnitId;
            var action = new InstallWorkloadsAction(scaleUnitId);
            await action.Execute();
        }
    }
}
