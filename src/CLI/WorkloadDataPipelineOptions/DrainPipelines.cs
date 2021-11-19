using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI.WorkloadDataPipelineOptions
{
    internal class DrainPipelines : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), Drain);
            var screen = new MultiSelectScreen(options, selectionHistory,
                $"Please select the environment(s) you want to drain\n" +
                $"Press enter to drain the workloads on all environments.\n",
                "\nWhich environment would you like to drain?: ");
            await CLIController.ShowScreen(screen);

            Console.WriteLine("Done\n");
        }

        public async Task Drain(int input, string selectionHistory)
        {
            var pipelineManager = new PipelineManager();
            await pipelineManager.DrainWorkloadDataPipelines();
        }
    }
}
