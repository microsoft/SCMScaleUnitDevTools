using System.Threading.Tasks;

namespace CLIFramework
{
    public interface CLIMenu
    {
        Task Show(int input, string selectionHistory);
    }
}
