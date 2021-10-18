using System;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            if (args.Length == 0)
            {
                await CLIController.Run(new RootMenu());
                return;
            }

            var argumentParser = new ArgumentParser();
            try
            {
                argumentParser.Parse(args);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return;
            }

            var deployer = new Deployer();

            if (argumentParser.CleanStorage)
            {
                await deployer.CleanStorage();
            }

            if (argumentParser.Deploy)
            {
                await deployer.Deploy();
            }
        }
    }
}
