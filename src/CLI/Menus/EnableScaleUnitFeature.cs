using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLI.Utilities;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace CLI.Menus
{
    internal class EnableScaleUnitFeature : DevToolMenu
    {
        protected List<IStep> availableSteps;
        private ScaleUnitInstance scaleUnit;

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PrintAvailableSteps);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to configure?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task PrintAvailableSteps(int input, string selectionHistory)
        {
            scaleUnit = GetSortedScaleUnits()[input - 1];
            var options = new List<CLIOption>();
            var stepGenerator = new StepGenerator(scaleUnit.ScaleUnitId);
            availableSteps = stepGenerator.GetSteps();

            foreach (IStep step in availableSteps)
            {
                options.Add(new CLIOption() { Name = step.Label(), Command = RunStep });
            }

            string helpMessage = $"Please select steps you would like to run by supplying the step numbers\n" +
                $"in a comma separated list, e.g. \"1,2,5,6\", or skip specific steps by supplying the negative step numbers\n" +
                $"to skip in a comma separated list, e.g. \"-3,-4\". You can also enter dash-separated intervals, e.g. \"1-4,6\".\n" +
                $"If nothing is entered every step will be executed.\n" +
                $"\nTasks to run for {scaleUnit.PrintableName()}:\n";

            CLIScreen screen = new MultiSelectScreen(options, selectionHistory, helpMessage, "\nSelect tasks to run/skip: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task RunStep(int input, string selectionHistory)
        {
            var action = new StepAction(scaleUnit.ScaleUnitId, availableSteps[input - 1]);
            await action.Execute();
        }
    }
}
