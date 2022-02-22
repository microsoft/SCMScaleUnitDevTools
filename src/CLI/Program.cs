using System;
using System.CommandLine;
using System.Threading.Tasks;
using CLI.Menus;
using CLI.Utilities;
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
            }
            else
            {
                var argumentHandler = new ArgumentHandler();
                RootCommand rootCommand = argumentHandler.BuildRootCommand();

                await rootCommand.InvokeAsync(args);
            }
        }
    }
}
