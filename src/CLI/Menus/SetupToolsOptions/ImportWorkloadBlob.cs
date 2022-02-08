using System.Threading.Tasks;
using CLIFramework;
using System.Collections.Generic;
using CLI.Actions;

namespace CLI.Menus.SetupToolsOptions
{
    internal class ImportWorkloadBlob : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), ImportWorkloadBlobFromSasToken);
            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the scale unit you would like to import workloads to:\n", "\nScale unit storage to import to: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task ImportWorkloadBlobFromSasToken(int input, string selectionHistory)
        {
            string sasToken = CLIController.EnterValuePrompt("Please paste in the blob SAS URL for the blob storage that the workloads should be copied from:");

            string scaleUnitId = GetScaleUnitId(input);
            var action = new ImportWorkloadBlobAction(scaleUnitId, sasToken);
            await action.Execute();
        }
    }
}
