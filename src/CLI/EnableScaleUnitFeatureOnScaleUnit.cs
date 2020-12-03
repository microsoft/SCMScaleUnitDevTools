using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    class EnableScaleUnitFeatureOnScaleUnit : EnableScaleUnitFeature
    {
        protected override List<Step> GetAvailableSteps()
        {
            List<Step> steps = base.GetAvailableSteps();

            StepFactory sf = new StepFactory();
            steps.AddRange(sf.GetStepsOfType<ScaleUnitStep>());

            return steps;
        }
    }
}
