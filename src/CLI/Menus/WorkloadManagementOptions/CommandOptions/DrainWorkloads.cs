using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class DrainWorkloads : ActionMenu
    {
        public override string Label => "Drain workload data pipelines";

        protected override IAction Action => new DrainWorkloadsAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new MultiSelectScreen(options, selectionHistory,
                $"Please select the environment(s) you want to drain\n" +
                $"Press enter to drain the workloads on all environments.\n",
                "\nWhich environment would you like to drain?: ");
            await CLIController.ShowScreen(screen);

            Console.WriteLine("Done\n");
        }
    }
}
