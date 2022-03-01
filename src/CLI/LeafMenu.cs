using System.Threading.Tasks;

namespace CLI
{
    internal abstract class LeafMenu : DevToolMenu
    {
        protected abstract Task PerformAction(int input, string selectionHistory);
    }
}
