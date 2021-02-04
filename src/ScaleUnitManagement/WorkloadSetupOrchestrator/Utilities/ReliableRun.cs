using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    static class ReliableRun
    {
        private static readonly string ErrorLogFilePath = Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
            "error.log");

        public static async Task Execute(Func<Task> command, string commandName)
        {
            int tryCount = 0;

            while (tryCount < Config.RetryCount)
            {
                try
                {
                    await command();
                    break;
                }
                catch (Exception e)
                {
                    LogErrorToFile(commandName, e);

                    if (tryCount == Config.RetryCount - 1)
                    {
                        Console.WriteLine(commandName + " failed. Retry count exceeded. Execution will not continue.");
                        throw;
                    }

                    Console.WriteLine($"{commandName} failed. Retrying... (find error output in {ErrorLogFilePath})");
                }

                tryCount++;
            }
        }

        private static void LogErrorToFile(string commandName, Exception exception)
        {
            File.AppendAllText(ErrorLogFilePath, $"[{DateTime.Now}] Error during {commandName}:\n{exception}\n\n");
        }
    }
}
