using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    internal class EnableScaleUnitFeature : DevToolMenu
    {
        protected List<IStep> availableSteps;

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(PrintAvailableStepsForScaleUnit);
            var screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to configure?: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task PrintAvailableStepsForScaleUnit(int input, string selectionHistory)
        {
            using var context = ScaleUnitContext.CreateContext(GetScaleUnitId(input - 1));
            if (ScaleUnitContext.GetScaleUnitId().Equals("@@"))
                await new EnableScaleUnitFeatureOnHub().PrintAvailableSteps(input, selectionHistory);
            else
                await new EnableScaleUnitFeatureOnScaleUnit().PrintAvailableSteps(input, selectionHistory);
        }

        protected virtual List<IStep> GetAvailableSteps()
        {
            var sf = new StepFactory();
            List<IStep> steps = sf.GetStepsOfType<ICommonStep>();
            return steps;
        }

        protected async Task PrintAvailableSteps(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            availableSteps = GetAvailableSteps();
            availableSteps.Sort((x, y) => x.Priority().CompareTo(y.Priority()));

            foreach (IStep step in availableSteps)
            {
                options.Add(new CLIOption() { Name = step.Label(), Command = RunStepsFromTask });
            }

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            var screen = new CLIScreen(options, selectionHistory, $"Task sequence to run for {scaleUnit.PrintableName()}\n", "\nSelect task to start from: ");
            await CLIController.ShowScreen(screen);
        }

        private async Task RunStepsFromTask(int input, string selectionHistory)
        {
            for (int i = input - 1; i < availableSteps.Count; i++)
            {
                try
                {
                    Console.WriteLine("Executing step: " + availableSteps[i].Label());
                    await availableSteps[i].Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error occurred while enabling scale unit feature:\n{ex}");

                    if (!CLIController.YesNoPrompt("Step " + availableSteps[i].Label() + " failed to complete successfuly. Do you want to continue? [y]: "))
                        break;
                }
            }

            Console.WriteLine("Done");
        }
    }
}
