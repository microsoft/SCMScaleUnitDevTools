using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StopServices : ICommonStep
    {
        public  string Label()
        {
            return "Stop services";
        }

        public  float Priority()
        {
            return 1F;
        }

        public  void Run()
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd = "Stop-Service -Name DynamicsAxBatch; iisreset /stop";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }
    }
}
