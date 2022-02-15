using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI.Actions
{
    internal class StepAction : ContextualAction
    {
        private readonly IStep step;

        public StepAction(string scaleUnitId, IStep step) : base(scaleUnitId)
        {
            this.step = step;
        }

        protected override async Task ExecuteInScaleUnitContext()
        {
            try
            {
                Console.WriteLine("Executing step: " + step.Label());
                await step.Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error occurred while enabling scale unit feature:\n{ex}");
            }
        }
    }
}
