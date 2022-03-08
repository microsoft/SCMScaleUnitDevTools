using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class StartWorkloads : ActionMenu
    {
        public override string Label => "Start workload data pipelines";

        protected override IAction Action => new StartWorkloadsAction(scaleUnitId);

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformScaleUnitAction);
            var screen = new MultiSelectScreen(options, selectionHistory,
                $"Please select the environment(s) you want to start.\n" +
                $"Press enter to start the workloads on all environments.\n",
                "\nWhich environment would you like to start?: ");
            await CLIController.ShowScreen(screen);

            Console.WriteLine("Done\n");
        }
    }
}
