using System;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class Step : IStep
    {
        public virtual string Label()
        {
            throw new NotImplementedException();
        }

        public virtual float Priority()
        {
            throw new NotImplementedException();
        }

        public virtual void Run()
        {
            throw new NotImplementedException();
        }
    }
}
