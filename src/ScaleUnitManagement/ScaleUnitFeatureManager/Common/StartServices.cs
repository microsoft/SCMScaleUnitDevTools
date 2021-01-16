using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StartServices : ICommonStep
    {
        public  string Label()
        {
            return "Start Services";
        }

        public  float Priority()
        {
            return 4F;
        }

        public  void Run()
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd = "Start-Service -Name DynamicsAxBatch; iisreset /start";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
