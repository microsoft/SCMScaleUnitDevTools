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
            const string UserName = "ScaleUnitManagement";
            string hubDb = Config.HubScaleUnit().AxDbName;
            var whitelisting = new AADAppWhitelisting();

            string interAOSAppId = Config.InterAOSAppId();
            const string InterAOSAppName = "ScaleUnits";
            whitelisting.UpdateAADAppClientTable(hubDb, UserName, InterAOSAppName, interAOSAppId);

            const string ScaleUnitAppName = "Scale Unit Management Tool";
            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            string scaleUnitAppId = scaleUnit.AuthConfiguration.AppId;
            whitelisting.UpdateAADAppClientTable(hubDb, UserName, ScaleUnitAppName, scaleUnitAppId);

            return Task.CompletedTask;
        }
    }
}
