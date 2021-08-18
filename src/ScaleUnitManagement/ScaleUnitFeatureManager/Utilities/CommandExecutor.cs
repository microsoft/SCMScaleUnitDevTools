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

        private readonly Process process;

        public CommandExecutor(string cmd)
        {
            process = BuildProcess("powershell", cmd);
        }

        public CommandExecutor(string executable, string arguments)
        {
            process = BuildProcess(executable, arguments);
        }

        public void RunCommand(params int[] successCodes)
        {
            RunProcess();

            if (successCodes.Length == 0)
            {
                successCodes = new[] { 0 };
            }

            if (!successCodes.Contains(process.ExitCode))
                throw new Exception($"Command: {process.StartInfo.FileName} {process.StartInfo.Arguments}");
        }

        private Process BuildProcess(string executable, string arguments)
        {
            return new Process
            {
                StartInfo = {
                    FileName = executable,
                    Arguments = arguments
                }
            };
        }

        public void AddOutputFile(string outputFile)
        {
            var outputStream = new StreamWriter(outputFile);
            process.StartInfo.RedirectStandardOutput = true;
            process.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                if (!String.IsNullOrEmpty(e.Data))
                {
                    outputStream.WriteLine(e.Data);
                }
            });
        }

        private void RunProcess()
        {
            process.Start();
            if (process.StartInfo.RedirectStandardOutput)
            {
                process.BeginOutputReadLine();
            }
            process.WaitForExit();
        }
    }
}
