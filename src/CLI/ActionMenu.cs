using System.Threading.Tasks;

namespace CLI
{
    internal abstract class ActionMenu : DevToolMenu
    {
        protected string scaleUnitId;

        protected async Task PerformScaleUnitAction(int input, string selectionHistory)
        {
            scaleUnitId = GetScaleUnitId(input);
            await Action.Execute();
        }

        protected abstract IAction Action { get; }
    }
}
