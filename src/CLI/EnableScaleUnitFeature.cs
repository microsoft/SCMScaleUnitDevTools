using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    class EnableScaleUnitFeature
    {
        protected List<IStep> availableSteps;
        private static List<ScaleUnitInstance> sortedScaleUnits;

        public static async Task SelectScaleUnit(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            sortedScaleUnits = Config.ScaleUnitInstances();
            sortedScaleUnits.Sort();

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnits)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = PrintAvailableStepsForScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to configure?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task PrintAvailableStepsForScaleUnit(int input, string selectionHistory)
        {
            using (var context = ScaleUnitContext.CreateContext(sortedScaleUnits[input - 1].ScaleUnitId))
            {
                if (sortedScaleUnits[input - 1].ScaleUnitId == "@@")
                    await new EnableScaleUnitFeatureOnHub().PrintAvailableSteps(input, selectionHistory);
                else
                    await new EnableScaleUnitFeatureOnScaleUnit().PrintAvailableSteps(input, selectionHistory);
            }
        }

        protected virtual List<IStep> GetAvailableSteps()
        {
            StepFactory sf = new StepFactory();
            List<IStep> steps = sf.GetStepsOfType<ICommonStep>();
            return steps;
        }

        protected async Task PrintAvailableSteps(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            availableSteps = GetAvailableSteps();
            availableSteps.Sort((x, y) => x.Priority().CompareTo(y.Priority()));

            foreach (IStep s in availableSteps)
            {
                options.Add(new CLIOption() { Name = s.Label(), Command = RunStepsFromTask });
            }

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            CLIScreen screen = new CLIScreen(options, selectionHistory, $"Task sequence to run for {scaleUnit.PrintableName()}\n", "\nSelect task to start from: ");
            await CLIMenu.ShowScreen(screen);
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

                    if (!CLIMenu.YesNoPrompt("Step " + availableSteps[i].Label() + " failed to complete successfuly. Do you want to continue? [y]: "))
                        break;
                }
            }

            Console.WriteLine("Done");
        }
    }
}
