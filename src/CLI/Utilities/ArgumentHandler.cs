using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Threading.Tasks;
using CLI.Actions;
using ScaleUnitManagement.Utilities;

namespace CLI.Utilities
{
    internal class ArgumentHandler
    {
        public RootCommand BuildRootCommand()
        {
            var singleBoxDeployOption = new Option<bool>(
            name: "--single-box-deploy",
            getDefaultValue: () => false,
            description: "Prepares and installs hub and spoke environments"
            );
            
            var hubDeployOption = new Option<bool>(
            name: "--hub-deploy",
            getDefaultValue: () => false,
            description: "Prepares and installs the hub"
            );

            var spokeDeployOption = new Option<bool>(
            name: "--spoke-deploy",
            getDefaultValue: () => false,
            description: "Prepares and installs the spoke"
            );

            var cleanStorageOption = new Option<bool>(
            name: "--clean-storage",
            getDefaultValue: () => false,
            description: "Cleans all Azure storage accounts for hub and spoke"
            );

            var drainPipelinesOption = new Option<bool>(
            name: "--drain-pipelines",
            getDefaultValue: () => false,
            description: "Drains pipelines between hub and spoke"
            );

            var startPipelinesOption = new Option<bool>(
            name: "--start-pipelines",
            getDefaultValue: () => false,
            description: "Starts pipelines between hub and spoke"
            );

            var rootCommand = new RootCommand {
                singleBoxDeployOption,
                hubDeployOption,
                spokeDeployOption,
                cleanStorageOption,
                drainPipelinesOption,
                startPipelinesOption
            };

            rootCommand.Description = "Scale Unit Management DevTools";

            rootCommand.SetHandler(async (
                bool singleBoxDeploy,
                bool hubDeploy,
                bool spokeDeploy,
                bool cleanStorage,
                bool drainPipelines,
                bool startPipelines
                ) =>
                {
                    await RunScripts(singleBoxDeploy, hubDeploy, spokeDeploy, cleanStorage, drainPipelines, startPipelines);
                }, 
                singleBoxDeployOption,
                hubDeployOption,
                spokeDeployOption,
                cleanStorageOption,
                drainPipelinesOption,
                startPipelinesOption
            );

            return rootCommand;
        }

        private async Task RunScripts(bool singleBoxDeploy, bool hubDeploy, bool spokeDeploy, bool cleanStorage, bool drainPipelines, bool startPipelines)
        {
            if ((singleBoxDeploy && hubDeploy) || (singleBoxDeploy && spokeDeploy) || (hubDeploy && spokeDeploy))
            {
                throw new Exception("You can only use one deploy option at a time.");
            }

            List<ScaleUnitInstance> sortedScaleUnitInstances = Config.ScaleUnitInstances();
            sortedScaleUnitInstances.Sort();

            if (cleanStorage)
            {
                foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
                {
                    Console.WriteLine($"\nCleaning environment on {scaleUnit.PrintableName()}");
                    var action = new CleanUpStorageAccountAction(scaleUnit.ScaleUnitId);
                    await action.Execute();
                }
            }

            if (singleBoxDeploy)
            {
                var deployer = new SingleBoxDeployer();
                await deployer.Deploy();
            }

            if (hubDeploy)
            {
                var deployer = new ScaleUnitDeployer(Config.HubScaleUnit());
                await deployer.Deploy();
            }

            if (spokeDeploy)
            {
                ScaleUnitInstance spoke = Config.NonHubScaleUnitInstances().First();
                var deployer = new ScaleUnitDeployer(spoke);
                await deployer.Deploy();
            }

            if (drainPipelines)
            {
                Console.WriteLine($"\nDraining all data pipelines");
                foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
                {
                    var action = new DrainPipelinesAction(scaleUnit.ScaleUnitId);
                    await action.Execute();
                }
                Console.WriteLine("Done.");
            }

            if (startPipelines)
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
