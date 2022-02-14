using System.Threading.Tasks;

namespace CLI
{
    internal interface IAction
    {
        Task Execute();
    }
}
