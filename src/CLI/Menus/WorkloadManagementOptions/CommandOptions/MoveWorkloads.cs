using System.Threading.Tasks;
using CLI.Actions;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class MoveWorkloads : LeafMenu
    {
        public override string Description => "Move all workloads to the hub";

        public override async Task Show(int input, string selectionHistory)
        {
            await PerformAction(input, selectionHistory);
        }

        protected async override Task PerformAction(int input, string selectionHistory)
        {
            await new MoveAllWorkloadsAction().Execute();
        }
    }
}
