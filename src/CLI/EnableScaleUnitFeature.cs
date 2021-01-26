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
        protected List<Step> AvailableSteps;

        public static async Task SelectScaleUnit(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();

            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
            {
                options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = PrintAvailableStepsForScaleUnit });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to configure?: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task PrintAvailableStepsForScaleUnit(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();

            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                if (scaleUnitInstances[input - 1].ScaleUnitId == "@@")
                    await new EnableScaleUnitFeatureOnHub().PrintAvailableSteps(input, selectionHistory);
                else
                    await new EnableScaleUnitFeatureOnScaleUnit().PrintAvailableSteps(input, selectionHistory);
            }
        }

        protected virtual List<Step> GetAvailableSteps()
        {
            StepFactory sf = new StepFactory();
            List<Step> steps = sf.GetStepsOfType<CommonStep>();
            return steps;
        }

        protected async Task PrintAvailableSteps(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();
            AvailableSteps = GetAvailableSteps();
            AvailableSteps.Sort((x, y) => x.Priority().CompareTo(y.Priority()));

            foreach (Step s in AvailableSteps)
            {
                options.Add(new CLIOption() { Name = s.Label(), Command = RunStepsFromTask });
            }

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            CLIScreen screen = new CLIScreen(options, selectionHistory, $"Tasks to run for scale unit {scaleUnit.PrintableName()} :\n", "\nSelect task to start from: ");
            await CLIMenu.ShowScreen(screen);
        }

        private Task RunStepsFromTask(int input, string selectionHistory)
        {
            for (int i = input - 1; i < AvailableSteps.Count; i++)
            {
                try
                {
                    Console.WriteLine("Executing step: " + AvailableSteps[i].Label());
                    AvailableSteps[i].Run();
                }
                catch (Exception)
                {
                    if (!CLIMenu.YesNoPrompt("Step " + AvailableSteps[i].Label() + " failed to complete successfuly. Do you want to continue? [y]: "))
                        break;
                }
            }

            Console.WriteLine("Done");
            return Task.CompletedTask;
        }
    }
}
