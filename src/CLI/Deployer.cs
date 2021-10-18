using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Hub;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal class Deployer
    {
        private readonly List<ScaleUnitInstance> sortedScaleUnitInstances;

        public Deployer()
        {
            sortedScaleUnitInstances = Config.ScaleUnitInstances();
            sortedScaleUnitInstances.Sort((s1, s2) => s1.ScaleUnitId.CompareTo(s2.ScaleUnitId));
        }

        public async Task Deploy()
        {
            if (!Config.UseSingleOneBox())
            {
                throw new Exception("You can only use automatic deployment in a SingleOneBox environment");
            }

            await InitializeEnvironments();
            await PrepareEnvironments();
            await InstallWorkloads();

            Console.WriteLine("\nHub and spoke environments have been deployed successfully!\n");
        }

        public async Task CleanStorage()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                Console.WriteLine($"\nCleaning environment on {scaleUnit.PrintableName()}");
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                var storageAccountManager = new StorageAccountManager();
                await storageAccountManager.CleanStorageAccount();
            }
        }

        private async Task InitializeEnvironments()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                Console.WriteLine($"\nInitializing environment on {scaleUnit.PrintableName()}");
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                List<IStep> steps = GetSteps();
                await RunSteps(steps);

                Console.WriteLine($"\nAll initialization steps completed for {scaleUnit.PrintableName()}");
            }
        }

        private List<IStep> GetSteps()
        {
            var sf = new StepFactory();
            List<IStep> steps = sf.GetStepsOfType<ICommonStep>();
            if (ScaleUnitContext.GetScaleUnitId() == "@@")
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

        private async Task RunSteps(List<IStep> steps)
        {
            for (int i = 0; i < steps.Count; i++)
            {
                try
                {
                    Console.WriteLine("\nExecuting step: " + steps[i].Label());
                    await steps[i].Run();
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error occurred while enabling scale unit feature:\n{ex}");
                }
            }
        }

        private async Task PrepareEnvironments()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                Console.WriteLine($"\nPreparing {scaleUnit.PrintableName()} for installation");
                if (scaleUnit.ScaleUnitId.Equals("@@"))
                {
                    await new HubConfigurationManager().Configure();
                }
                else
                {
                    await new ScaleUnitConfigurationManager().Configure();
                }
            }
        }

        private async Task InstallWorkloads()
        {
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                Console.WriteLine($"\nInstalling workloads on {scaleUnit.PrintableName()}");
                if (scaleUnit.IsHub())
                {
                    await new HubWorkloadInstaller().Install();
                }
                else
                {
                    await new ScaleUnitWorkloadInstaller().Install();
                }
            }
        }
    }
}


