using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StopServices : ICommonStep
    {
        public string Label()
        {
            return "Stop services";
        }

        public float Priority()
        {
            return 1F;
        }

        public Task Run()
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            ServiceHelper.StopService(scaleUnit.BatchServiceName(), timeout: TimeSpan.FromMinutes(1));

            IISAdministrationHelper.StopAppPoolAndSite(scaleUnit.AppPoolName(), scaleUnit.SiteName());

            return Task.CompletedTask;
        }
    }
}
