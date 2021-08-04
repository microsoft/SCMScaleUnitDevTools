using System.Threading.Tasks;
using ScaleUnitManagement.WorkloadSetupOrchestrator;
using System;

namespace CLI
{
    class DeleteWorkloads
    {
        public static async Task DeleteWorkloadsFromHub(int input, string selectionHistory)
        {
            try
            {
                Console.WriteLine("Deleting all workloads from the hub");
                await WorkloadDeleter.DeleteWorkloadsFromHub();
                Console.WriteLine("done");
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to delete workloads:\n{ex}");
            }
        }
    }
}
