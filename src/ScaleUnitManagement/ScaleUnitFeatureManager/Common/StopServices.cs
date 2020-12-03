using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class StopServices : CommonStep
    {
        public override string Label()
        {
            return "Stop services";
        }

        public override float Priority()
        {
            return 1F;
        }

        public override void Run()
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
