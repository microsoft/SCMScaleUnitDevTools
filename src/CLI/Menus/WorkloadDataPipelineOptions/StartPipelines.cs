using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadDataPipelineOptions
{
    internal class StartPipelines : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), Start);
            var screen = new MultiSelectScreen(options, selectionHistory,
                $"Please select the environment(s) you want to start.\n" +
                $"Press enter to start the workloads on all environments.\n",
                "\nWhich environment would you like to start?: ");
            await CLIController.ShowScreen(screen);

            Console.WriteLine("Done\n");
        }

        private async Task Start(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new StartPipelinesAction(scaleUnitId);
            await action.Execute();
        }
    }
}
