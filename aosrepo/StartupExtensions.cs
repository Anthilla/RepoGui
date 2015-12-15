using Owin;
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace aosrepo {
    public static class X509CertificateMiddleware {
        public static void UseCertificate(this IAppBuilder app, string path = null) {
            if (path == null)
                return;
            if (!File.Exists(path)) {
                throw new Exception($"File not found {path}");
            }
            try {
                var certificate = new X509Certificate2(path);
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
