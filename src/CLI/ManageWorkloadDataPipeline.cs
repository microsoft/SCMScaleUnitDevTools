using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal class ManageWorkloadDataPipeline : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>()
            {
                Option("Drain all workload data pipelines", DrainAllPipelines),
                Option("Start all workload data pipelines", StartAllPipelines),
            };

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }

        public async Task DrainAllPipelines(int input, string selectionHistory)
        {
            foreach (ScaleUnitInstance scaleUnit in GetSortedScaleUnits())
            {
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                var pipelineManager = new PipelineManager();
                await pipelineManager.DrainWorkloadDataPipelines();
            }
            Console.WriteLine("Done.");
        }

        public async Task StartAllPipelines(int input, string selectionHistory)
        {
            foreach (ScaleUnitInstance scaleUnit in GetSortedScaleUnits())
            {
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                var pipelineManager = new PipelineManager();
                await pipelineManager.StartWorkloadDataPipelines();
            }
            Console.WriteLine("Done.");
        }
    }
}
