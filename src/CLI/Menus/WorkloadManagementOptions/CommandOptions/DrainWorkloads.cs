using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class DrainWorkloads : LeafMenu
    {
        public override string Description => "Drain workload data pipelines";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new MultiSelectScreen(options, selectionHistory,
                $"Please select the environment(s) you want to drain\n" +
                $"Press enter to drain the workloads on all environments.\n",
                "\nWhich environment would you like to drain?: ");
            await CLIController.ShowScreen(screen);

            Console.WriteLine("Done\n");
        }

        protected async override Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new DrainWorkloadsAction(scaleUnitId);
            await action.Execute();
        }
    }
}
