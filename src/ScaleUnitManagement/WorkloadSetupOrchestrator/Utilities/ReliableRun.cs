using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;
using System.Net;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    public static class ReliableRun
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
                catch (AOSClientError e)
                {
                    HandleError(e, commandName, tryCount);
                    string tsg = SuggestTSG(e);
                    if (!string.IsNullOrEmpty(tsg))
                    {
                        Console.WriteLine(tsg);
                        throw;
                    }
                    if (!IsRetryableAOSCall(e.StatusCode))
                    {
                        throw;
                    }
                }

                tryCount++;
                Console.WriteLine("\nRetrying...");
            }
        }

        private static void HandleError(Exception e, string commandName, int tryCount)
        {
            LogErrorToFile(commandName, e);
            Console.WriteLine($"\n{commandName} failed with exception: {e.Message} \nFind complete error output in {ErrorLogFilePath}");

            if (tryCount == Config.RetryCount - 1)
            {
                Console.WriteLine(commandName + " failed. Retry count exceeded. Execution will not continue.");
                throw e;
            }
        }

        private static bool IsRetryableAOSCall(HttpStatusCode statusCode)
        {
            Console.WriteLine((int)statusCode);
            return (int)statusCode >= 300 &&
                statusCode != HttpStatusCode.BadRequest &&
                statusCode != HttpStatusCode.NotFound &&
                statusCode != HttpStatusCode.Unauthorized &&
                statusCode != HttpStatusCode.Forbidden;
        }

        private static string SuggestTSG(Exception e)
        {
            string spokeConfiguredBeforeHubError = "The provided Dynamics.AX.Application.ScaleUnitEnvironmentConfiguration is invalid";
            if (e.Message.Contains(spokeConfiguredBeforeHubError))
            {
                return "\nDid you try to configure the spoke before configuring the hub?\n";
            }

            string axNotRunningError = "No connection could be made because the target machine actively refused it";
            if (e.Message.Contains(axNotRunningError))
            {
                return "\nIs the AX batch service running?\n";
            }

            return "";
        }

        private static void LogErrorToFile(string commandName, Exception exception)
        {
            File.AppendAllText(ErrorLogFilePath, $"[{DateTime.Now}] Error during {commandName}:\n{exception}\n\n");
        }
    }
}
