using System.Threading.Tasks;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public interface IStep
    {
        float Priority();
        string Label();
        Task Run();
    }
}
