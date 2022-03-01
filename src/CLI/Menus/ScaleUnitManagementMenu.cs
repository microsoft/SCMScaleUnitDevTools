using System.Collections.Generic;
using CLI.Menus.ScaleUnitManagementOptions;

namespace CLI.Menus
{
    internal class ScaleUnitManagementMenu : NavigationMenu
    {
        public override string Description => "Scale unit management";

        protected override List<DevToolMenu> SubMenus()
        {
            return new List<DevToolMenu> {
                new EnableScaleUnitFeature(),
                new ConfigureEnvironment(),
                new DisableScaleUnitFeature(),
                new UpdateScaleUnitId()
            };
        }
    }
}
