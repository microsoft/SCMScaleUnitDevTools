using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI
{
    public class Program
    {
        public static int Main(string[] args)
        {
            try
            {
                CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                Console.Error.WriteLine("\r\n\r\nPress any key to exit. . .");
                Console.ReadKey();
                return 740; // ERROR_ELEVATION_REQUIRED
            }

            MainAsync(args).GetAwaiter().GetResult();
            return 0;
        }

        public static async Task MainAsync(string[] args)
        {
            var enableScaleUnitFeatureOption = new CLIOption() { Name = "Initialize the hybrid topology", Command = EnableScaleUnitFeature.SelectScaleUnit };
            var configureEnvironmentOption = new CLIOption() { Name = "Prepare environments for workload installation", Command = ConfigureEnvironment.Show };
            var installWorkloadsOption = new CLIOption() { Name = "Install workloads", Command = InstallWorkloads.Show };
            var workloadsInstallationStatusOption = new CLIOption() { Name = "Show workloads installation status", Command = WorkloadsInstallationStatus.Show };

            var options = new List<CLIOption>() { enableScaleUnitFeatureOption, configureEnvironmentOption, installWorkloadsOption, workloadsInstallationStatusOption };

            var screen = new CLIScreen(options, "Home", "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static bool HasAdministratorPrivileges()
        {
            var id = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(id);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
