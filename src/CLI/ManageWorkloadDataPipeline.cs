using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal static class ManageWorkloadDataPipeline
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var drainWorkloadsOption = new CLIOption() { Name = "Drain all workload data pipelines", Command = DrainAllPipelines };
            var startWorkloadsOption = new CLIOption() { Name = "Start all workload data pipelines", Command = StartAllPipelines };

            var options = new List<CLIOption>() { drainWorkloadsOption, startWorkloadsOption };

            var screen = new CLIScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }

        public static async Task DrainAllPipelines(int input, string selectionHistory)
        {
            var scaleUnits = Config.ScaleUnitInstances();
            foreach (ScaleUnitInstance scaleUnit in scaleUnits)
            {
                using (var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId))
                {
                    var pipelineManager = new PipelineManager();
                    await pipelineManager.DrainWorkloadDataPipelines();
                }
            }
            Console.WriteLine("Done.");
        }

        public static async Task StartAllPipelines(int input, string selectionHistory)
        {
            var scaleUnits = Config.ScaleUnitInstances();
            foreach (ScaleUnitInstance scaleUnit in scaleUnits)
            {
                using (var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId))
                {
                    var pipelineManager = new PipelineManager();
                    await pipelineManager.StartWorkloadDataPipelines();
                }
            }
            Console.WriteLine("Done.");
        }
    }
}
