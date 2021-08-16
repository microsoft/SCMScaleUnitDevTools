using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    internal class SetupTools
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            options.Add(new CLIOption() { Name = "Sync DB", Command = SyncDB.Show });
            options.Add(new CLIOption() { Name = "Clean up Azure storage blobs for a scale unit", Command = CleanUpBlobs.Show });

            var screen = new CLIScreen(options, selectionHistory, "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
