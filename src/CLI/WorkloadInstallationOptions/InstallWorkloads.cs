using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.WorkloadInstallationOptions
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
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubWorkloadInstaller().Install();
            else
                await new ScaleUnitWorkloadInstaller().Install();
        }
    }
}
