using System.Collections.Generic;
using CLI.Menus.WorkloadManagementOptions.CommandOptions;

namespace CLI.Menus.WorkloadManagementOptions
{
    internal class CommandMenu : NavigationMenu
    {
        public override string Label => "Command options";

        protected override List<DevToolMenu> SubMenus()
        {
            return new List<DevToolMenu> {
                new InstallWorkloads(),
                new MoveWorkloads(),
                new DrainWorkloads(),
                new StartWorkloads(),
                new UpgradeWorkloadsDefinition(),
                new PerformEmergencyTransitionToHub(),
                new DeleteWorkloads(),
                new ImportWorkloadBlob(),
            };
        }
    }
}
