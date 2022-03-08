using System.Collections.Generic;
using CLI.Menus.DatabaseManagementOptions;

namespace CLI.Menus
{
    internal class DatabaseManagementMenu : NavigationMenu
    {
        public override string Label => "Database management";

        protected override List<DevToolMenu> SubMenus()
        {
            return new List<DevToolMenu>
            {
                new CleanUpStorageAccount(),
                new SyncDB(),
            };
        }
    }
}
