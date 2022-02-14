using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;

namespace CLI.Actions
{
    internal class SyncDBAction : ContextualAction
    {
        public SyncDBAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override Task ExecuteInScaleUnitContext()
        {
            try
            {
                new RunDBSync().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to run DbSync:\n{ex}");
            }
            return Task.CompletedTask;
        }
    }
}
