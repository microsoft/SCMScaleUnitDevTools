using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    class SetupTools
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            options.Add(new CLIOption() { Name = "Sync DB", Command = SyncDB.Show });

            var screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }
    }
}
