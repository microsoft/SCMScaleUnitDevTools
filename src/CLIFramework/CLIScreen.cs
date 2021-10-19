using System.Collections.Generic;
using System.Threading.Tasks;

namespace CLIFramework
{
    public abstract class CLIScreen
    {
        public CLIScreen previousScreen = null;
        public List<CLIOption> options = null;
        public List<CLIOption> navigationOptions = null;
        public string selectionHistory = "";
        public string infoBeforeOptions = "";
        public string infoAfterOptions = "";
        public string inputValidationError = "";
        public CLIScreenState state = CLIScreenState.Incomplete;

        public CLIScreen(List<CLIOption> options, string selectionHistory, string infoBeforeOptions = "", string infoAfterOptions = "")
        {
            this.options = options;
            this.selectionHistory = selectionHistory;
            this.infoBeforeOptions = infoBeforeOptions;
            this.infoAfterOptions = infoAfterOptions;
            this.navigationOptions = new List<CLIOption>();
        }

        public abstract Task PerformAction(string input);

        protected async Task RunCommand(List<CLIOption> options, int number)
        {
            await options[number - 1].Command(number, string.IsNullOrEmpty(selectionHistory) ? options[number - 1].Name : selectionHistory + " -> " + options[number - 1].Name);
        }
    }
}
