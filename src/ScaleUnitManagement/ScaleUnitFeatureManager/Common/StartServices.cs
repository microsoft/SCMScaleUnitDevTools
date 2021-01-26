using System;
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

        public void Run()
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            string cmd = $@"
                 Start-Service -Name {scaleUnit.BatchServiceName()};
                 .$env:systemroot\System32\inetsrv\appcmd.exe start apppool /apppool.name:{scaleUnit.AppPoolName()};
                 .$env:systemroot\System32\inetsrv\appcmd.exe start site /site.name:{scaleUnit.SiteName()}; 
            ";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
