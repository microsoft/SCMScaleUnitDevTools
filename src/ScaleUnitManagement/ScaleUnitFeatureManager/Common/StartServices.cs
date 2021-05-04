using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StartServices : ICommonStep
    {
        public string Label()
        {
            return "Start Services";
        }

        public float Priority()
        {
            return 4F;
        }

        public Task Run()
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            ServiceHelper.StartService(scaleUnit.BatchServiceName(), timeout: TimeSpan.FromMinutes(1));

            IISAdministrationHelper.StartAppPoolAndSite(scaleUnit.AppPoolName(), scaleUnit.SiteName());

            return Task.CompletedTask;
        }
    }
}
