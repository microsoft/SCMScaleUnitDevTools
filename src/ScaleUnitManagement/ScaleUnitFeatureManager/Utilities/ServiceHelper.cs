using System;
using System.Linq;
using System.ServiceProcess;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public static class ServiceHelper
    {
        public static void StartService(string serviceName, TimeSpan timeout) =>
            RunServiceAction(serviceName, svc =>
            {
                Console.WriteLine($"Starting service {serviceName}..");

                if (svc.Status == ServiceControllerStatus.Running)
                {
                    Console.WriteLine($"{serviceName} already started");
                    return;
                }

                svc.Start();
                svc.WaitForStatus(ServiceControllerStatus.Running, timeout);

                Console.WriteLine($"Successfully started service {serviceName}");
            });

        public static void StopService(string serviceName, TimeSpan timeout) =>
            RunServiceAction(serviceName, svc =>
            {
                Console.WriteLine($"Stopping service {serviceName}..");

                if (svc.Status != ServiceControllerStatus.Running)
                {
                    Console.WriteLine($"{serviceName} already stopping or stopped");
                    return;
                }

                svc.Stop();
                svc.WaitForStatus(ServiceControllerStatus.Stopped, timeout);

                Console.WriteLine($"Successfully stopped service {serviceName}");
            });

        private static void RunServiceAction(string serviceName, Action<ServiceController> action)
        {
            if (!ServiceExists(serviceName))
            {
                return;
            }

            using (var service = new ServiceController(serviceName))
            {
                action(service);
            }
        }

        private static bool ServiceExists(string serviceName)
        {
            return ServiceController.GetServices().FirstOrDefault(s => s.ServiceName == serviceName) != null;
        }
    }
}
