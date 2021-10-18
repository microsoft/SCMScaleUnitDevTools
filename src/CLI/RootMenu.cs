using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    internal class RootMenu : DevToolMenu
    {

        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>() {
                Option("Initialize the hybrid topology", new EnableScaleUnitFeature().Show),
                Option("Prepare environments for workload installation", new ConfigureEnvironment().Show),
                Option("Show workload installation options", new WorkloadInstallationMenu().Show),
                Option("Show workload movement options", new WorkloadMovementMenu().Show),
                Option("Upgrade workloads definition", new UpgradeWorkloadsDefinition().UpgradeAllWorkloadDefinitions),
                Option("Show workload data pipeline management options", new ManageWorkloadDataPipeline().Show),
                Option("Delete workloads from environment", new DeleteWorkloads().Show),
                Option("Setup tools", new SetupTools().Show),
            };

            var screen = new SingleSelectScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
