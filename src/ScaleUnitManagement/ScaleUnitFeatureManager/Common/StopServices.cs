using System;
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

        public void Run()
        {
            CheckForAdminAccess.ValidateCurrentUserIsProcessAdmin();

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string cmd = $@"
                Stop-Service -Name {scaleUnit.BatchServiceName()};
                .$env:systemroot\System32\inetsrv\appcmd.exe stop apppool /apppool.name:{scaleUnit.AppPoolName()};
                .$env:systemroot\System32\inetsrv\appcmd.exe stop site /site.name:{scaleUnit.SiteName()}; 
            ";

            CommandExecutor ce = new CommandExecutor();

            try
            {
                ce.RunCommand(cmd);
            }
            catch (Exception)
            {
                if (!Config.UseSingleOneBox())
                    throw;
            }
        }
    }
}
