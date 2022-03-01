using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class ImportWorkloadBlob : LeafMenu
    {
        public override string Description => "Import workloads from Azure storage blob with SAS url";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the scale unit you would like to import workloads to:\n", "\nScale unit storage to import to: ");
            await CLIController.ShowScreen(screen);
        }

        protected async override Task PerformAction(int input, string selectionHistory)
        {
            string sasToken = CLIController.EnterValuePrompt("Please paste in the blob SAS URL for the blob storage that the workloads should be copied from:");

            string scaleUnitId = GetScaleUnitId(input);
            var action = new ImportWorkloadBlobAction(scaleUnitId, sasToken);
            await action.Execute();
        }
    }
}
