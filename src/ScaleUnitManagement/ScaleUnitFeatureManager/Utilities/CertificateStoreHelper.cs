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
            X509Certificate2 certificate = RetrieveCertificateFromStore(certificateSubject);

            if (certificate == null)
            {
                Console.Error.WriteLine($"SSL cert not found in cert store {StoreLocation.LocalMachine}. Attempting to create it.");
                EnsureCertificateWithSubjectExists(certificateSubject);
                certificate = RetrieveCertificateFromStore(certificateSubject);
            }

            if (certificate == null)
            {
                throw new Exception($"SSL cert not found in cert store {StoreLocation.LocalMachine} and couldn't create certificate.");
            }

            return certificate.GetCertHash();
        }

        internal static X509Certificate2 RetrieveCertificateFromStore(string certificateSubject)
        {
            var certCollection = GetLocalMachineCertificates();
            X509Certificate2 certificate = null;

            foreach (var cert in certCollection.Cast<X509Certificate2>().Where(cert => cert.Subject.Equals($"CN={certificateSubject}")))
            {
                certificate = cert;
            }

            return certificate;
        }

        private static X509Certificate2Collection GetLocalMachineCertificates()
        {
            var localMachineStore = new X509Store(StoreLocation.LocalMachine);
            localMachineStore.Open(OpenFlags.ReadOnly);
            var certificates = localMachineStore.Certificates;
            localMachineStore.Close();
            return certificates;
        }

        internal static void EnsureCertificateWithSubjectExists(string certificateSubject)
        {
            if (!CheckForAdminAccess.IsCurrentProcessAdmin())
            {
                throw new NotSupportedException("Please run the tool from a shell that is running as administrator.");
            }

            string cmd = "New-SelfSignedCertificate -NotBefore (Get-Date) -NotAfter (Get-Date).AddYears(1)"
                + " -Subject " + CommandExecutor.Quotes + certificateSubject + CommandExecutor.Quotes
                + " -KeyAlgorithm " + CommandExecutor.Quotes + "RSA" + CommandExecutor.Quotes + " -KeyLength 2048"
                + " -HashAlgorithm " + CommandExecutor.Quotes + "SHA256" + CommandExecutor.Quotes
                + " -CertStoreLocation " + CommandExecutor.Quotes + @"Cert:\LocalMachine\My" + CommandExecutor.Quotes
                + " -KeyUsage KeyEncipherment -FriendlyName " + CommandExecutor.Quotes + "HTTPS development certificate" + CommandExecutor.Quotes
                + " -TextExtension @(" + CommandExecutor.Quotes + "2.5.29.19={critical}{text}" + CommandExecutor.Quotes + "," + CommandExecutor.Quotes + "2.5.29.37={critical}{text}" + CommandExecutor.Quotes
                + "," + CommandExecutor.Quotes + "2.5.29.37={critical}{text}1.3.6.1.5.5.7.3.1" + CommandExecutor.Quotes + "," + CommandExecutor.Quotes + "2.5.29.17={critical}{text}DNS=" + certificateSubject + CommandExecutor.Quotes + ")";

            CommandExecutor ce = new CommandExecutor();
            ce.RunCommand(cmd);
        }

        internal static string PersonalCertificateStoreName => "MY";
    }
}
