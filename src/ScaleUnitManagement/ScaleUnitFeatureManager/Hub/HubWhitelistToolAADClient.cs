using System;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Hub
{
    public sealed class HubWhitelistToolAADClient : IHubStep
    {
        private AddToolClientToSysAADClientTable AddToolClientToSysAADClientTable
        => new AddToolClientToSysAADClientTable();

        public string Label()
        {
            return this.AddToolClientToSysAADClientTable.Label();
        }

        public float Priority()
        {
            return this.AddToolClientToSysAADClientTable.Priority();
        }

        public void Run()
        {
            this.AddToolClientToSysAADClientTable.Run(Config.AxDbName());
        }
    }
}
