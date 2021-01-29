using System;

namespace ScaleUnitManagement.Utilities
{
    public class ScaleUnitContext : IDisposable
    {
        private static ScaleUnitContext instance = null;
        private readonly string scaleUnitId;

        private ScaleUnitContext(string scaleUnitId)
        {
            this.scaleUnitId = scaleUnitId;
            instance = this;
        }

        public static ScaleUnitContext CreateContext(string scaleUnitId)
        {
            if (instance != null)
                throw new Exception("Can't create a ScaleUnitContext while an active context already exists");

            return new ScaleUnitContext(scaleUnitId);
        }

        public static string GetScaleUnitId()
        {
            return instance.scaleUnitId;
        }

        public void Dispose()
        {
            instance = null;
        }
    }
}
