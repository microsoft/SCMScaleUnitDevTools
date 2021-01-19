using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace ScaleUnitManagement.ScaleUnitFeatureManager.Utilities
{
    internal static class CertificateStoreHelper
    {
        internal static byte[] GetCertificateFromLocalMachineStore(string certificateSubject)
        {
            var certCollection = GetLocalMachineCertificates();
            X509Certificate2 certificate = null;

            foreach (var cert in certCollection.Cast<X509Certificate2>().Where(cert => cert.Subject.Equals($"CN={certificateSubject}")))
            {
                certificate = cert;
            }

            if (certificate == null)
            {
                throw new Exception($"SSL cert not found in cert store {StoreLocation.LocalMachine}");
            }

            return certificate.GetCertHash();
        }

        private static X509Certificate2Collection GetLocalMachineCertificates()
        {
            var localMachineStore = new X509Store(StoreLocation.LocalMachine);
            localMachineStore.Open(OpenFlags.ReadOnly);
            var certificates = localMachineStore.Certificates;
            localMachineStore.Close();
            return certificates;
        }

        internal static string PersonalCertificateStoreName => "MY";
    }
}
