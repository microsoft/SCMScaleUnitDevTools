using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class CommandExecutor
    {
        public static string Quotes = "\"\"\"";

        public void RunCommand(string cmd, List<int> successCodes)
        {
            var process = ExecuteCommand(cmd);

            if (!successCodes.Contains(process.ExitCode))
                throw new Exception("Command: " + cmd);
        }

        public void RunCommand(string cmd)
        {
            var process = ExecuteCommand(cmd);

            if (process.ExitCode != 0)
                throw new Exception("Command: " + cmd);
        }

        private static Process ExecuteCommand(string cmd)
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
            return process;
        }
    }
}
