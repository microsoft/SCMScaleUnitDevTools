using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CLIFramework;

namespace CLI
{
    public abstract class NavigationMenu : DevToolMenu
    {
        public override async Task Show(int input, string selectionHistory)
        {
            var options = SubMenus().Select(menu => Option(menu.Description, menu.Show)).ToList();

            var screen = new SingleSelectScreen(options, selectionHistory, "Please select the operation you want to perform:\n", "\nOperation to perform: ");
            await CLIController.ShowScreen(screen);
        }

        protected abstract List<DevToolMenu> SubMenus();
    }
}
