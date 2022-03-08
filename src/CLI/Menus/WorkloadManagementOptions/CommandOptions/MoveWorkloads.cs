using System.Threading.Tasks;
using CLI.Actions;

namespace CLI.Menus.WorkloadManagementOptions.CommandOptions
{
    internal class MoveWorkloads : ActionMenu
    {
        public override string Label => "Move all workloads to the hub";

        protected override IAction Action => new MoveAllWorkloadsAction();

        public override async Task Show(int input, string selectionHistory)
        {
            await Action.Execute();
        }
    }
}
