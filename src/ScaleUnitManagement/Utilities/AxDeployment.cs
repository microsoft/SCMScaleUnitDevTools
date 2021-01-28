using System;
using System.Diagnostics;

namespace ScaleUnitManagement.Utilities
{
    public class AxDeployment
    {
        private static AxDeployment instance = null;
        protected string ApplicationVersion = "";

        protected AxDeployment()
        {
            instance = this;
        }

        private static AxDeployment Instance()
        {
            if (instance == null)
                new AxDeployment();

            return instance;
        }

        private static string GetApplicationVersion()
        {
            if (!String.IsNullOrEmpty(Instance().ApplicationVersion))
                return Instance().ApplicationVersion;

            ScaleUnitInstance scaleUnit = Config.FindScaleUnitWithId(ScaleUnitContext.GetScaleUnitId());

            FileVersionInfo versionInfo = FileVersionInfo.GetVersionInfo($@"{scaleUnit.ServiceVolume}\AOSService\PackagesLocalDirectory\ApplicationSuite\bin\Microsoft.Dynamics.AX.DemandPlanning.Azure.dll");
            Instance().ApplicationVersion = versionInfo.FileVersion;

            return Instance().ApplicationVersion;
        }

        public static bool IsApplicationVersionLaterThan(string version)
        {
            string[] deployedApplicationVersion = GetApplicationVersion().Split('.');
            string[] compareToVersion = version.Split('.');

            for (int i = 0; i < deployedApplicationVersion.Length && i < compareToVersion.Length; i++)
            {
                if (Int32.Parse(deployedApplicationVersion[i]) < Int32.Parse(compareToVersion[i]))
                    return true;
                else if (Int32.Parse(deployedApplicationVersion[i]) > Int32.Parse(compareToVersion[i]))
                    return false;
            }

            return false;
        }
    }
}
