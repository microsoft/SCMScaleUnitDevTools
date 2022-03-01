using System;
using System.Threading.Tasks;
using ScaleUnitManagement.DatabaseManager;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public sealed class AddToolToHubSysAADClientTable : IHubStep
    {
        public string Label()
        {
            return "Add CLI tool App to SysAADClientTable";
        }

        public float Priority()
        {
            return 6F;
        }

        public Task Run()
        {
            string hubDb = Config.HubScaleUnit().AxDbName;
            var allowListing = new AADAppAllowListing();

            try
            {
                string interAOSUserName =
                    AxDbManager.UserExists("ScaleUnitPipeline")
                    ? "ScaleUnitPipeline"
                    : "Admin";

                const string InterAOSAppName = "ScaleUnits";
                string interAOSAppId = Config.InterAOSAppId();
                allowListing.UpdateAADAppClientTable(hubDb, interAOSUserName, InterAOSAppName, interAOSAppId);

                const string ScaleUnitUserName = "ScaleUnitManagement";
                const string ScaleUnitAppName = "Scale Unit Management Tool";
                ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
                string scaleUnitAppId = scaleUnit.AuthConfiguration.AppId;
                allowListing.UpdateAADAppClientTable(hubDb, ScaleUnitUserName, ScaleUnitAppName, scaleUnitAppId);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine("\nFailed to add CLI tool App to SysAADClientTable.");
            }

            return Task.CompletedTask;
        }
    }
}
