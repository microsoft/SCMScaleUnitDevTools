using System;
using System.Threading.Tasks;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.WorkloadSetupOrchestrator.Utilities
{
    static class ReliableRun
    {
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
                    if (tryCount == Config.RetryCount - 1)
                    {
                        Console.WriteLine(commandName + " failed. Retry count exceeded. Execution will not continue.");
                        throw e;
                    }

                    Console.WriteLine(commandName + " failed. Retrying...");
                }

                tryCount++;
            }
        }
    }
}
