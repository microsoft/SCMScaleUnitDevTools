using System;
using System.IO;
using ScaleUnitManagement.ScaleUnitFeatureManager.Utilities;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Common
{
    public class RunDBSync
    {
        public string Label()
        {
            return "Run DB Sync";
        }

        public float Priority()
        {
            return 3F;
        }

        public void Run(bool isScaleUnitFeatureEnabled = true)
        {
            Console.WriteLine("Executing DbSync");

            try
            {
                ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

                string dbSyncTool = Path.Combine(scaleUnit.ServiceVolume, @"AOSService\PackagesLocalDirectory\bin\syncengine.exe");
                string metaBinariesPath = Path.Combine(scaleUnit.ServiceVolume, @"AOSService\PackagesLocalDirectory");
                string outputFile = $"{scaleUnit.AxDbName}_DbSync.log";
                string scaleUnitMnemonic = isScaleUnitFeatureEnabled ? scaleUnit.ScaleUnitId : "";

                // If ScaleUnit feature is disabled, we should have the triggers isEnabled option as true to allow DBsync to remove the triggers.
                // Otherwise, it will fail to remove the triggers.
                string args =
                    "-syncmode=\"fullall\""
                    + $" -metadatabinaries=\"{metaBinariesPath}\""
                    + $" -connect=\"Data Source=localhost;Initial Catalog={scaleUnit.AxDbName};Integrated Security=True;Enlist=True;Application Name=AXVSSDK\""
                    + " -verbosity=\"Diagnostic\""
                    + " -scaleUnitOptionsAsJson=\"{\"IsScaleUnitFeatureEnabled\": " + $"{isScaleUnitFeatureEnabled.ToString().ToLower()}, \"scaleUnitMnemonics\":" + $"'{scaleUnitMnemonic}'" + " }\""
                    + " -triggerOptionsAsJson=\"{\"IsEnabled\": true}\"";

                Console.WriteLine($"{dbSyncTool} {args} > {outputFile}");

                Console.WriteLine("\nDBSync started at: " + DateTime.UtcNow + ", this will take approximately 30 minutes.\n");

                CommandExecutor ce = new CommandExecutor(dbSyncTool, args);
                ce.AddOutputFile(outputFile);
                ce.RunCommand();

                Console.WriteLine("\nDBSync finished \n");
            }

            catch (Exception)
            {
                Console.WriteLine("\nDBSync failed \n");
                throw;
            }
        }
    }
}
