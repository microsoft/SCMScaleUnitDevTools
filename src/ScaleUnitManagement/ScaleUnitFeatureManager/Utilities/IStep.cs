namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public interface IStep
    {
        float Priority();
        string Label();
        void Run();
    }
}
