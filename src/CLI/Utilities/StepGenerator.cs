using System.Collections.Generic;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Hub;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI.Utilities
{
    internal class StepGenerator
    {
        private readonly string scaleUnitId;

        public StepGenerator(string scaleUnitId)
        {
            this.scaleUnitId = scaleUnitId;
        }

        public List<IStep> GetSteps()
        {
            var sf = new StepFactory();
            List<IStep> steps = sf.GetStepsOfType<ICommonStep>();
            if (scaleUnitId == "@@")
            {
                steps.AddRange(sf.GetStepsOfType<IHubStep>());
            }
            else
            {
                steps.AddRange(sf.GetStepsOfType<IScaleUnitStep>());
            }
            steps.Sort((x, y) => x.Priority().CompareTo(y.Priority()));
            return steps;
        }
    }
}
