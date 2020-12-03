using System;
using System.Collections.Generic;
using System.Text;

namespace CLIFramework
{
    public class CLIScreen
    {
        public CLIScreen PreviousScreen = null;
        public List<CLIOption> Options = null;
        public string SelectionHistory = "";
        public string InfoBeforeOptions = "";
        public string InfoAfterOptions = "";
        public string InputValidationError = "";
        public CLIScreenState state = CLIScreenState.Incomplete;

        public CLIScreen(List<CLIOption> options, string selectionHistory, string infoBeforeOptions = "", string infoAfterOptions = "")
        {
            this.Options = options;
            this.SelectionHistory = selectionHistory;
            this.InfoBeforeOptions = infoBeforeOptions;
            this.InfoAfterOptions = infoAfterOptions;
        }
    }
}
