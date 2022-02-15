using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Actions;
using ScaleUnitManagement.Utilities;

namespace CLI.Utilities
{
    internal class ArgumentHandler
    {
        internal async Task RunScripts(ArgumentParser argumentParser)
        {
            List<ScaleUnitInstance> sortedScaleUnitInstances = Config.ScaleUnitInstances();
            sortedScaleUnitInstances.Sort();

            if (argumentParser.CleanStorage)
            {
                foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
                {
                    Console.WriteLine($"\nCleaning environment on {scaleUnit.PrintableName()}");
                    var action = new CleanUpStorageAccountAction(scaleUnit.ScaleUnitId);
                    await action.Execute();
                }
            }

            if (argumentParser.SingleBoxDeploy)
            {
                var deployer = new SingleBoxDeployer();
                await deployer.Deploy();
            }

            if (argumentParser.HubDeploy)
            {
                var deployer = new ScaleUnitDeployer(Config.HubScaleUnit());
                await deployer.Deploy();
            }

            if (argumentParser.SpokeDeploy)
            {
                ScaleUnitInstance spoke = Config.NonHubScaleUnitInstances().First();
                var deployer = new ScaleUnitDeployer(spoke);
                await deployer.Deploy();
            }

            if (argumentParser.DrainPipelines)
            {
                Console.WriteLine($"\nDraining all data pipelines");
                foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
                {
                    var action = new DrainPipelinesAction(scaleUnit.ScaleUnitId);
                    await action.Execute();
                }
                Console.WriteLine("Done.");
            }

            if (argumentParser.StartPipelines)
            {
                Console.WriteLine($"\nStarting all data pipelines");
                foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
                {
                    var action = new StartPipelinesAction(scaleUnit.ScaleUnitId);
                    await action.Execute();
                }
                Console.WriteLine("Done.");
            }
        }
    }
}
