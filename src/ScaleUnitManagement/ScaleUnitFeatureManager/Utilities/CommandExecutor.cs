using System;
using System.Diagnostics;
using System.IO;


namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class CommandExecutor
    {
        public static string Quotes = "\"\"\"";

        public void RunCommand(string cmd)
        {
            var process = new Process
            {
                StartInfo = {
                    FileName = "powershell",
                    Arguments = cmd
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
                throw new Exception("Command: " + cmd);
        }
    }
}
