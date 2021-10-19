using System.Threading.Tasks;
using CLIFramework;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using System.Collections.Generic;

namespace CLI.SetupToolsOptions
{
    internal class DisableScaleUnitFeature : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<CLIOption> options = SelectScaleUnitOptions(GetSortedScaleUnits(), RunDisableScaleUnitFeature);
            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to disable scale unit feature on?: ");
            await CLIController.ShowScreen(screen);
        }

        private Task RunDisableScaleUnitFeature(int input, string selectionHistory)
        {
            try
            {
                new StopServices().Run();
                using (var webConfig = new WebConfig())
                {
                    SharedWebConfig.Configure(webConfig, isScaleUnitFeatureEnabled: false);
                }
                new RunDBSync().Run(isScaleUnitFeatureEnabled: false);
                new StartServices().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to disable scale unit feature:\n{ex}");
            }
            return Task.CompletedTask;
        }
    }
}
