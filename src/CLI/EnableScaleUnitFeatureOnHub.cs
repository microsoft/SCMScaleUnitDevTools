using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.Hub;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    class EnableScaleUnitFeatureOnHub : EnableScaleUnitFeature
    {
        protected  List<IStep> GetAvailableSteps()
        {
            List<IStep> steps = base.GetAvailableSteps();

            StepFactory sf = new StepFactory();
            steps.AddRange(sf.GetStepsOfType<IHubStep>());

            return steps;
        }
    }
}
