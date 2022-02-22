using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLI.Actions;
using CLI.Menus;
using CLI.Utilities;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace CLI
{
    public class Program
    {
        /// <summary>
        /// Scale Unit Management DevTools
        /// </summary>
        /// <param name="singleBoxDeploy">Prepares and installs hub and spoke environments on the current machine</param>
        /// <param name="hubDeploy">Prepares and installs the hub on the current machine</param>
        /// <param name="spokeDeploy">Prepares and installs the spoke on the current machine</param>
        /// <param name="cleanStorage">Cleans all Azure storage accounts for hub and spoke</param>
        /// <param name="drainPipelines">Drains pipelines between hub and spoke</param>
        /// <param name="startPipelines">Starts pipelines between hub and spoke</param>
        public static async Task Main(bool singleBoxDeploy,
                bool hubDeploy,
                bool spokeDeploy,
                bool cleanStorage,
                bool drainPipelines,
                bool startPipelines
            )
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            if (!singleBoxDeploy && !hubDeploy && !spokeDeploy && !cleanStorage && !drainPipelines && !startPipelines )
            {
                await CLIController.Run(new RootMenu());
                return;
            }
            
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
