using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace CLI.Actions
{
    internal class DisableScaleUnitFeatureAction : ContextualAction
    {
        public DisableScaleUnitFeatureAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override Task ExecuteInScaleUnitContext()
        {
            try
            {
                new StopServices().Run();
                using (var webConfig = new WebConfig())
                {
                    SharedWebConfig.Configure(webConfig, isScaleUnitFeatureEnabled: false);
                }
                new RunDBSync().Run(isScaleUnitFeatureEnabled: false);
                new StartServices().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to disable scale unit feature:\n{ex}");
            }
            return Task.CompletedTask;
        }
    }
}
