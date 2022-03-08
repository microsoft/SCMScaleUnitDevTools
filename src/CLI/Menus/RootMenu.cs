using System.Collections.Generic;

namespace CLI.Menus
{
    internal class RootMenu : NavigationMenu
    {
        public override string Label => "Home";

        protected override List<DevToolMenu> SubMenus()
        {
            return new List<DevToolMenu> {
                new ScaleUnitManagementMenu(), 
                new WorkloadManagementMenu(),
                new DatabaseManagementMenu()
            };
        }
    }
}
