using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.Hub;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    class EnableScaleUnitFeatureOnHub : EnableScaleUnitFeature
    {
        protected override List<Step> GetAvailableSteps()
        {
            List<Step> steps = base.GetAvailableSteps();

            StepFactory sf = new StepFactory();
            steps.AddRange(sf.GetStepsOfType<HubStep>());

            return steps;
        }
    }
}
