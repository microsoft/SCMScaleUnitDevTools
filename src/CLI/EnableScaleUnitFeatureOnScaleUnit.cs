using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    class EnableScaleUnitFeatureOnScaleUnit : EnableScaleUnitFeature
    {
        protected  List<IStep> GetAvailableSteps()
        {
            List<IStep> steps = base.GetAvailableSteps();

            StepFactory sf = new StepFactory();
            steps.AddRange(sf.GetStepsOfType<IScaleUnitStep>());

            return steps;
        }
    }
}
