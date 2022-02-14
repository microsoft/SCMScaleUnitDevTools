using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CLI.Actions;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace CLI.Utilities
{
    internal class Deployer
    {
        private readonly List<ScaleUnitInstance> sortedScaleUnitInstances;

        public Deployer()
        {
            sortedScaleUnitInstances = Config.ScaleUnitInstances();
            sortedScaleUnitInstances.Sort();
        }

        public async Task Deploy()
        {
            if (!Config.UseSingleOneBox())
            {
                throw new Exception("You can only use automatic deployment in a SingleOneBox environment");
            }

            await InitializeEnvironments();
            await ConfigureEnvironments();
            await InstallWorkloads();

            Console.WriteLine("\nHub and spoke environments have been deployed successfully!\n");
        }

        public async Task CleanStorage()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                Console.WriteLine($"\nCleaning environment on {scaleUnit.PrintableName()}");
                var action = new CleanUpStorageAccountAction(scaleUnit.ScaleUnitId);
                await action.Execute();
            }
        }

        public async Task DrainAllPipelines()
        {
            Console.WriteLine($"\nDraining all data pipelines");
            foreach (ScaleUnitInstance scaleUnit in Config.ScaleUnitInstances())
            {
                var action = new DrainPipelinesAction(scaleUnit.ScaleUnitId);
                await action.Execute();
            }
            Console.WriteLine("Done.");
        }

        public async Task StartAllPipelines()
        {
            Console.WriteLine($"\nStarting all data pipelines");
            foreach (ScaleUnitInstance scaleUnit in Config.ScaleUnitInstances())
            {
                var action = new StartPipelinesAction(scaleUnit.ScaleUnitId);
                await action.Execute();
            }
            Console.WriteLine("Done.");
        }

        private async Task InitializeEnvironments()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
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
        }

        private async Task ConfigureEnvironments()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                Console.WriteLine($"\nPreparing {scaleUnit.PrintableName()} for installation");
                var action = new ConfigureEnvironmentAction(scaleUnit.ScaleUnitId);
                await action.Execute();
            }
        }

        private async Task InstallWorkloads()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                Console.WriteLine($"\nInstalling workloads on {scaleUnit.PrintableName()}");
                var action = new InstallWorkloadsAction(scaleUnit.ScaleUnitId);
                await action.Execute();
            }
        }
    }
}


