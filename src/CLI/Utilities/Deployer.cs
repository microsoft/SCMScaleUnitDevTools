using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace CLI.Utilities
{
    internal abstract class Deployer
    {
        public abstract Task Deploy();

        protected async Task InitializeEnvironments(ScaleUnitInstance scaleUnit)
        {
            Console.WriteLine($"\nInitializing environment on {scaleUnit.PrintableName()}");
            var stepGenerator = new StepGenerator(scaleUnit.ScaleUnitId);
            List<IStep> steps = stepGenerator.GetSteps();
            foreach (IStep step in steps)
            {
                var action = new StepAction(scaleUnit.ScaleUnitId, step);
                await action.Execute();
            }

            Console.WriteLine($"\nAll initialization steps completed for {scaleUnit.PrintableName()}");
        }

        protected async Task ConfigureEnvironments(ScaleUnitInstance scaleUnit)
        {
            Console.WriteLine($"\nPreparing {scaleUnit.PrintableName()} for installation");
            var action = new ConfigureEnvironmentAction(scaleUnit.ScaleUnitId);
            await action.Execute();
        }

        protected async Task InstallWorkloads(ScaleUnitInstance scaleUnit)
        {
            Console.WriteLine($"\nInstalling workloads on {scaleUnit.PrintableName()}");
            var action = new InstallWorkloadsAction(scaleUnit.ScaleUnitId);
            await action.Execute();
        }
    }
}
