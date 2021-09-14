using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.Hub;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    internal class EnableScaleUnitFeatureOnHub : EnableScaleUnitFeature
    {
        protected override List<IStep> GetAvailableSteps()
        {
            List<IStep> steps = base.GetAvailableSteps();

            var sf = new StepFactory();
            steps.AddRange(sf.GetStepsOfType<IHubStep>());

            return steps;
        }
    }
}
