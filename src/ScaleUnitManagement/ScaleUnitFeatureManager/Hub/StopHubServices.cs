using System;
using System.Collections.Generic;
using System.Text;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public sealed class StopHubServices : IHubStep
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
                batchName: Config.HubBatchName,
                appPoolName: Config.HubAppPoolName,
                siteName: Config.HubAppPoolName);
        }
    }
}
