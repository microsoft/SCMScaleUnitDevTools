using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

// TODO: Needs to differentiate between batches.

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StopServices
    {
        public string Label()
        {
            return "Stop services";
        }

        public float Priority()
        {
            return 1F;
        }

        public void Run(string batchName, string appPoolName, string siteName)
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd = $@"
                Stop-Service -Name {batchName};
                .$env:systemroot\System32\inetsrv\appcmd.exe stop apppool /apppool.name:{appPoolName};
                .$env:systemroot\System32\inetsrv\appcmd.exe stop site /site.name:{siteName}; 
            ";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
