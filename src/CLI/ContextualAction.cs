using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    internal abstract class ContextualAction : IAction
    {
        private readonly string scaleUnitId;

        public ContextualAction(string scaleUnitId)
        {
            this.scaleUnitId = scaleUnitId;
        }

        public async Task Execute()
        {
            using var context = ScaleUnitContext.CreateContext(scaleUnitId);
            await ExecuteInScaleUnitContext();
        }

        protected abstract Task ExecuteInScaleUnitContext();
    }
}
