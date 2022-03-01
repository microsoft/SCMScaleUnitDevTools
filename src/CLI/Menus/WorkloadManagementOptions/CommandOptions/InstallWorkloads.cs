using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class InstallWorkloads : LeafMenu
    {
        public override string Description => "Install workloads";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Install workloads on:\n", "\nEnvironment to install the workloads on?: ");
            await CLIController.ShowScreen(screen);
        }

        protected async override Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new InstallWorkloadsAction(scaleUnitId);
            await action.Execute();
        }
    }
}
