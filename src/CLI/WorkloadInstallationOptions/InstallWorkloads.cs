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
            var options = SelectScaleUnitOptions(InstallWorkloadsForScaleUnit);
            var screen = new CLIScreen(options, selectionHistory, "Install workloads on:\n", "\nEnvironment to install the workloads on?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task InstallWorkloadsForScaleUnit(int input, string selectionHistory)
        {
            using var context = ScaleUnitContext.CreateContext(GetScaleUnitId(input - 1));
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
                await new HubWorkloadInstaller().Install();
            else
                await new ScaleUnitWorkloadInstaller().Install();
        }
    }
}
