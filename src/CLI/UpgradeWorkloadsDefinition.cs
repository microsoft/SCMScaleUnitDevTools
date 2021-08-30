using System;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal class UpgradeWorkloadsDefinition
    {
        public static async Task UpgradeAllWorkloadDefinitions(int input, string selectionHistory)
        {
            if (!CLIMenu.YesNoPrompt("You are about to upgrade the workload definitions on all scale units. Please make sure that all data pipelines have been drained. Do you wish to continue? [y]: "))
                return;

            var scaleUnits = Config.ScaleUnitInstances();
            foreach (var scaleUnit in scaleUnits)
            {
                using (var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId))
                {
                    var workloadDefinitionManager = new WorkloadDefinitionManager();
                    await workloadDefinitionManager.UpgradeWorkloadsDefinition();
                }
            }
            Console.WriteLine("Done");
        }
    }
}
