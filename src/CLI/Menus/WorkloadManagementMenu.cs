using System.Collections.Generic;
using CLI.Menus.WorkloadManagementOptions;

namespace CLI.Menus
{
    internal class WorkloadManagementMenu : NavigationMenu
    {
        public override string Description => "Workload management";

        protected override List<DevToolMenu> SubMenus()
        {
            return new List<DevToolMenu> {
                new CommandMenu(),
                new QueryMenu(),
            };
        }
    }
}
