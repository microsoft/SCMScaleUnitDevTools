using System;
using System.Threading.Tasks;
using ScaleUnitManagement.ScaleUnitFeatureManager.Common;
using ScaleUnitManagement.Utilities;

namespace CLI.Actions
{
    internal class UpdateScaleUnitIdAction : ContextualAction
    {
        public UpdateScaleUnitIdAction(string scaleUnitId) : base(scaleUnitId)
        {
        }

        protected override Task ExecuteInScaleUnitContext()
        {
            try
            {
                new StopServices().Run();
                using (var webConfig = new WebConfig())
                {
                    SharedWebConfig.Configure(webConfig);
                }
                new StartServices().Run();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"An error occured while trying to update scale unit id:\n{ex}");
            }
            return Task.CompletedTask;
        }
    }
}
