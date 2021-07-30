using System.Collections.Generic;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.ScaleUnitFeatureManager.Hub;
using ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit;

namespace CLI
{
    class SyncDB
    {
        public static async Task Show(int input, string selectionHistory)
        {
            var options = new List<CLIOption>();


            if (Config.UseSingleOneBox())
            {
                List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
                scaleUnitInstances.Sort();

                foreach (ScaleUnitInstance scaleUnit in scaleUnitInstances)
                {
                    options.Add(new CLIOption() { Name = scaleUnit.PrintableName(), Command = SyncSpokeDB });
                }
            }
            else
            {
                options.Add(new CLIOption() { Name = "Hub", Command = SyncHubDB });
            }

            var screen = new CLIScreen(options, "Home", "Please select the database you would like to sync:\n", "\nDatabase to sync: ");
            await CLIMenu.ShowScreen(screen);
        }

        private static async Task SyncHubDB(int input, string selectionHistory)
        {
            HubDBSync hubDBSync = new HubDBSync();
            await hubDBSync.Run();
        }
        private static async Task SyncSpokeDB(int input, string selectionHistory)
        {
            List<ScaleUnitInstance> scaleUnitInstances = Config.ScaleUnitInstances();
            scaleUnitInstances.Sort();
            ScaleUnitDBSync scaleUnitDBSync = new ScaleUnitDBSync();
            using (var context = ScaleUnitContext.CreateContext(scaleUnitInstances[input - 1].ScaleUnitId))
            {
                await scaleUnitDBSync.Run();
            }
        }
    }
}
