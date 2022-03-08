using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class ImportWorkloadBlob : ActionMenu
    {
        private string sasToken;

        public override string Label => "Import workloads from Azure storage blob with SAS url";
        protected override IAction Action => new ImportWorkloadBlobAction(scaleUnitId, sasToken);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), ImportWorkloadsFromSAS);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the scale unit you would like to import workloads to:\n", "\nScale unit storage to import to: ");
            await CLIController.ShowScreen(screen);
        }

        protected async Task ImportWorkloadsFromSAS(int input, string selectionHistory)
        {
            sasToken = CLIController.EnterValuePrompt("Please paste in the blob SAS URL for the blob storage that the workloads should be copied from:");
            await PerformScaleUnitAction(input, selectionHistory);
        }
    }
}
