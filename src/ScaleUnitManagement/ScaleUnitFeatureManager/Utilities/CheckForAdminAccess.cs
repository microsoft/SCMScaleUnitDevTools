using System;
using System.Runtime.InteropServices;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    public class CheckForAdminAccess
    {
        const string NoAdminMessage = "Access Denied!\r\n" +
                    "This tool must be run with administrator permissions.\r\n" +
                    "Please start the tool in an administrator command prompt.";

        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();

        public static void ValidateCurrentUserIsProcessAdmin()
        {
            if (!IsUserAnAdmin())
            {
                Console.Error.WriteLine(NoAdminMessage);
                Console.Error.WriteLine("\r\n\r\nPress any key to exit. . .");
                Console.ReadKey();
                Environment.Exit(740);
            }
        }
    }
}
