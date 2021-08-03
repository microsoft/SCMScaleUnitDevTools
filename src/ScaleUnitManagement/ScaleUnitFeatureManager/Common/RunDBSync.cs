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

        public void Run()
        {
            Console.WriteLine("Executing DbSync");

            try
            {
                ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

                string dbSyncTool = Path.Combine(scaleUnit.ServiceVolume, @"AOSService\PackagesLocalDirectory\bin\syncengine.exe");
                string metaBinariesPath = Path.Combine(scaleUnit.ServiceVolume, @"AOSService\PackagesLocalDirectory");

                string cmd =
                    dbSyncTool
                    + " -syncmode=" + CommandExecutor.Quotes + "fullall" + CommandExecutor.Quotes
                    + " -metadatabinaries=" + CommandExecutor.Quotes + metaBinariesPath + CommandExecutor.Quotes
                    + " -connect=" + CommandExecutor.Quotes + $"Data Source=localhost;Initial Catalog={scaleUnit.AxDbName};Integrated Security=True;Enlist=True;Application Name=AXVSSDK" + CommandExecutor.Quotes
                    + " -verbosity=" + CommandExecutor.Quotes + "Diagnostic" + CommandExecutor.Quotes
                    + " -scaleUnitOptionsAsJson=" + CommandExecutor.Quotes + "{" + CommandExecutor.Quotes + "IsScaleUnitFeatureEnabled" + CommandExecutor.Quotes + ": true, " + CommandExecutor.Quotes + "scaleUnitMnemonics" + CommandExecutor.Quotes + ":" + $"'{scaleUnit.ScaleUnitId}'" + " }" + CommandExecutor.Quotes
                    + " -triggerOptionsAsJson=" + CommandExecutor.Quotes + "{" + CommandExecutor.Quotes + "IsEnabled" + CommandExecutor.Quotes + ": true}" + CommandExecutor.Quotes
                    + $" > {scaleUnit.AxDbName}_DbSync.log";

                Console.WriteLine(cmd);

                Console.WriteLine("\nDBSync started at: " + DateTime.UtcNow + ", this will take approximately 30 minutes.\n");

                CommandExecutor ce = new CommandExecutor();
                ce.RunCommand(cmd);

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
