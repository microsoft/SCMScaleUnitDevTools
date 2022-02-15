using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace CLI.Utilities
{
    internal class SingleBoxDeployer : Deployer
    {
        public override async Task Deploy()
        {
            List<ScaleUnitInstance> sortedScaleUnitInstances = Config.ScaleUnitInstances();
            sortedScaleUnitInstances.Sort();

            if (!Config.UseSingleOneBox())
            {
                throw new Exception("You can only use automatic deployment in a SingleOneBox environment");
            }

            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                await InitializeEnvironments(scaleUnit);
            }
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                await ConfigureEnvironments(scaleUnit);
            }
            foreach (ScaleUnitInstance scaleUnit in sortedScaleUnitInstances)
            {
                await InstallWorkloads(scaleUnit);
            }

            Console.WriteLine("\nHub and spoke environments have been deployed successfully!\n");
        }
    }
}
