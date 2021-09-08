using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    internal class EnableScaleUnitFeatureOnScaleUnit : EnableScaleUnitFeature
    {
        protected override List<IStep> GetAvailableSteps()
        {
            var steps = base.GetAvailableSteps();

            var sf = new StepFactory();
            steps.AddRange(sf.GetStepsOfType<IScaleUnitStep>());

            return steps;
        }
    }
}
