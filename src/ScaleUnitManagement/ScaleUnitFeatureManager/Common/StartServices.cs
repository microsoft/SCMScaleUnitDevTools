using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

// TODO: Needs to differentiate between batches.

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StartServices
    {
        public string Label()
        {
            return "Start Services";
        }

        public float Priority()
        {
            return 4F;
        }

        public void Run(string batchName, string appPoolName, string siteName)
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd =
                 $@"Start-Service -Name {batchName}; " +
                 $@"%SYSTEMROOT%\System32\inetsrv\appcmd start apppool /apppool.name:{appPoolName}; " +
                 $@"%SYSTEMROOT%\System32\inetsrv\appcmd start site /site.name:{siteName}; ";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
