using System;
using System.Threading.Tasks;
using CLIFramework;
using ScaleUnitManagement.Utilities;
using ScaleUnitManagement.WorkloadSetupOrchestrator;

namespace CLI
{
    internal class UpgradeWorkloadsDefinition
    {
        public async Task UpgradeAllWorkloadDefinitions(int input, string selectionHistory)
        {
            if (!CLIController.YesNoPrompt("You are about to upgrade the workload definitions on all scale units. Do you wish to continue? [y]: "))
                return;

            var scaleUnits = Config.ScaleUnitInstances();
            foreach (var scaleUnit in scaleUnits)
            {
                using var context = ScaleUnitContext.CreateContext(scaleUnit.ScaleUnitId);
                var workloadDefinitionManager = new WorkloadDefinitionManager();
                await workloadDefinitionManager.UpgradeWorkloadsDefinition();
            }
            Console.WriteLine("Done");
        }
    }
}
