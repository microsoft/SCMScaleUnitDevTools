using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Menus.WorkloadDataPipelineOptions;
using CLIFramework;

namespace CLI.Menus
{
    internal class ManageWorkloadDataPipeline : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>()
            {
                Option("Drain workload data pipelines", new DrainPipelines().Show),
                Option("Start workload data pipelines", new StartPipelines().Show),
            };

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you would like to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }
    }
}
