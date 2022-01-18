using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.SetupToolsOptions;
using CLIFramework;

namespace CLI
{
    internal class SetupTools : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>()
            {
                Option("Sync DB", new SyncDB().Show),
                Option("Disable scale unit feature and remove triggers", new DisableScaleUnitFeature().Show),
                Option("Update scale unit id", new UpdateScaleUnitId().Show),
                Option("Clean up Azure storage account for a scale unit", new CleanUpStorageAccount().Show),
                Option("Import workloads from Azure storage blob with SAS url", new ImportWorkloadBlob().Show),
            };

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
