using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using CLIFramework;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class UpgradeWorkloadsDefinition : LeafMenu
    {
        public override string Description => "Upgrade workloads definition";

        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), PerformAction);
            var screen = new MultiSelectScreen(options, selectionHistory,
                $"Please select the environment(s) you want to upgrade the workload definitions on\n" +
                $"Press enter to upgrade the workloads on all environments.\n",
                "\nWhich environment would you like to upgrade the workloads on?: ");
            await CLIController.ShowScreen(screen);

            Console.WriteLine("Done\n");
        }

        protected async override Task PerformAction(int input, string selectionHistory)
        {
            string scaleUnitId = GetScaleUnitId(input);
            var action = new UpgradeWorkloadsDefinitionAction(scaleUnitId);
            await action.Execute();
        }
    }
}
