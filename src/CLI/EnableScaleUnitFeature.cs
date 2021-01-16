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
        protected List<IStep> AvailableSteps;

        public static async Task Show(int input, string selectionHistory)
        {
            CLIOption enableScaleUnitFeatureHubOption = new CLIOption() { Name = "Hub", Command = new EnableScaleUnitFeatureOnHub().PrintAvailableSteps };
            CLIOption enableScaleUnitFeatureOnScaleUnitOption = new CLIOption() { Name = "Scale Unit", Command = new EnableScaleUnitFeatureOnScaleUnit().PrintAvailableSteps };

            var options = new List<CLIOption>() { enableScaleUnitFeatureHubOption, enableScaleUnitFeatureOnScaleUnitOption };

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Type of environment to setup:\n", "\nWould you like to setup this environment as the hub or a scale unit?: ");
            await CLIMenu.ShowScreen(screen);
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
            AvailableSteps = GetAvailableSteps();
            AvailableSteps.Sort((x, y) => x.Priority().CompareTo(y.Priority()));

            foreach (IStep s in AvailableSteps)
            {
                options.Add(new CLIOption() { Name = s.Label(), Command = RunStepsFromTask });
            }

            CLIScreen screen = new CLIScreen(options, selectionHistory, "Tasks to run:\n", "\nSelect task to start from: ");
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
                    if (!CLIMenu.YesNoPrompt("IStep " + AvailableSteps[i].Label() + " failed to complete successfuly. Do you want to continue? [y]: "))
                        break;
                }
            }

            Console.WriteLine("Done");
            return Task.CompletedTask;
        }
    }
}
