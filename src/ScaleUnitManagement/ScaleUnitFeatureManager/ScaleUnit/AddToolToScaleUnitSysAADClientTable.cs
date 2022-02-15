using System;
using System.Threading.Tasks;
using ScaleUnitManagement.DatabaseManager;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public sealed class AddToolToScaleUnitSysAADClientTable : IScaleUnitStep
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
            const string ScaleUnitAppName = "Scale Unit Management Tool";

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());
            string scaleUnitAppId = scaleUnit.AuthConfiguration.AppId;
            string dbName = scaleUnit.AxDbName;

            var allowListing = new AADAppAllowListing();

            try
            {
                allowListing.UpdateAADAppClientTable(dbName, UserName, ScaleUnitAppName, scaleUnitAppId);
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
