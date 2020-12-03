using System.Runtime.InteropServices;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    class CheckForAdminAccess
    {
        [DllImport("shell32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool IsUserAnAdmin();

        public static bool IsCurrentProcessAdmin()
        {
            return IsUserAnAdmin();
        }
    }
}
