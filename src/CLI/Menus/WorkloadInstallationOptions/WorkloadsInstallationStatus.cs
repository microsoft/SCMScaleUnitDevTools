using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.WorkloadInstallationOptions
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
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubWorkloadInstaller().InstallationStatus();
            else
                await new ScaleUnitWorkloadInstaller().InstallationStatus();
        }
    }
}
