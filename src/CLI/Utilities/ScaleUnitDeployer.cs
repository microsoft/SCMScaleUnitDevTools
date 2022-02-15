using System;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace CLI.Utilities
{
    internal class ScaleUnitDeployer : Deployer
    {
        private readonly ScaleUnitInstance scaleUnit;

        public ScaleUnitDeployer(ScaleUnitInstance scaleUnit) : base()
        {
            this.scaleUnit = scaleUnit;
        }

        public override async Task Deploy()
        {
            Console.WriteLine($"Deploying scale unit {scaleUnit.PrintableName()}");

            await InitializeEnvironments(scaleUnit);
            await ConfigureEnvironments(scaleUnit);
            await InstallWorkloads(scaleUnit);

            Console.WriteLine($"\nScale unit {scaleUnit.PrintableName()} has been deployed successfully!\n");
        }
    }
}
