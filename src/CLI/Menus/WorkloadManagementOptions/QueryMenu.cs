using System.Collections.Generic;
using CLI.Menus.WorkloadManagementOptions.QueryOptions;

namespace CLI.Menus.WorkloadManagementOptions
{
    internal class QueryMenu : NavigationMenu
    {
        public override string Label => "Query options";

        protected override List<DevToolMenu> SubMenus()
        {
            return new List<DevToolMenu> {
                new WorkloadsInstallationStatus(),
                new WorkloadMovementStatus(),
            };
        }
    }
}
