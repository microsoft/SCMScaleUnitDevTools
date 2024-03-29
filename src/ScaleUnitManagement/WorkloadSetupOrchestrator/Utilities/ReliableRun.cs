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
                catch (Exception exception)
                {
                    if (TryDiagnoseError(exception, commandName) || !IsRetryableAOSCall(exception))
                    {
                        throw;
                    }

                    if (tryCount == Config.RetryCount - 1)
                    {
                        Console.WriteLine("Retry count exceeded. Execution will not continue.");
                        throw;
                    }

                    tryCount++;
                    Console.WriteLine("\nRetrying...");
                }
            }
        }

        private static bool TryDiagnoseError(Exception exception, string commandName)
        {
            LogErrorToFile(commandName, exception);
            Console.WriteLine($"\n{commandName} failed with exception: {exception.Message} \nFind complete error output in {ErrorLogFilePath}");

            string tsg = SuggestTSG(exception);
            if (!string.IsNullOrEmpty(tsg))
            {
                Console.WriteLine(tsg);
                return true;
            }
            else return false;
        }

        private static bool IsRetryableAOSCall(Exception exception)
        {
            if (!(exception is AOSClientError))
            {
                return false;
            }
            else
            {
                var aosClientError = exception as AOSClientError;
                HttpStatusCode statusCode = aosClientError.StatusCode;
                return (int)statusCode >= 300 &&
                    statusCode != HttpStatusCode.BadRequest &&
                    statusCode != HttpStatusCode.NotFound &&
                    statusCode != HttpStatusCode.Unauthorized &&
                    statusCode != HttpStatusCode.Forbidden;
            }
        }

        private static string SuggestTSG(Exception exception)
        {
            string invalidConfigurationError = "The provided Dynamics.AX.Application.ScaleUnitEnvironmentConfiguration is invalid";
            if (exception.Message.Contains(invalidConfigurationError))
            {
                return "\nThere was a problem with the configuration. This could be caused by:\n" +
                    "1. Trying to configure the spoke before configuring the hub.\n" +
                    "2. Empty or missing fields in the configuration file. Make sure that the configuration file is filled out correctly, see details on the devTool wiki page.\n" +
                    "3. Other errors that make this scale unit unable to communicate with the hub.\n";
            }

            string axNotRunningError = "No connection could be made because the target machine actively refused it";
            if (exception.Message.Contains(axNotRunningError))
            {
                return "\nIs the IIS and the W3WP instance for AX running? If not, try running the \"Start services\" initialization step.\n";
            }

            return "";
        }

        private static void LogErrorToFile(string commandName, Exception exception)
        {
            File.AppendAllText(ErrorLogFilePath, $"[{DateTime.Now}] Error during {commandName}:\n{exception}\n\n");
        }
    }
}
