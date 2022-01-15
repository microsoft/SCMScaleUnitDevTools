using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace CLI.SetupToolsOptions
{
    internal class UpdateScaleUnitId : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> sortedNonHubScaleUnits = Config.NonHubScaleUnitInstances();
            sortedNonHubScaleUnits.Sort();
            List<CLIOption> options = SelectScaleUnitOptions(sortedNonHubScaleUnits, RunUpdateScaleunitId);

            var screen = new SingleSelectScreen(options, selectionHistory, "Environments:\n", "\nWhich environment would you like to update the scale unit id of?: ");
            await CLIController.ShowScreen(screen);
        }

        private Task RunUpdateScaleunitId(int input, string selectionHistory)
        {
            try
            {
                new StopServices().Run();
                using (var webConfig = new WebConfig())
                {
                    SharedWebConfig.Configure(webConfig);
                }
                new StartServices().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to update scale unit id:\n{ex}");
            }
            return Task.CompletedTask;
        }
    }
}
