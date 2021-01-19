using System;
using System.Collections.Generic;
using System.Text;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.ScaleUnit
{
    public sealed class StopScaleUnitServices : IScaleUnitStep
    {
        private StopServices StopServices => new StopServices();

        public string Label()
        {
            return this.StopServices.Label();
        }

        public float Priority()
        {
            return this.StopServices.Priority();
        }

        public void Run()
        {
            this.StopServices.Run(
                batchName: Config.ScaleUnitBatchName,
                appPoolName: Config.ScaleUnitAppPoolName,
                siteName: Config.ScaleUnitAppPoolName);
        }
    }
}
