using Owin;
using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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
                //var ip = ServerConfiguration.GetServerIp();
                //var port = ServerConfiguration.GetServerPort();
                //var certificate = new X509Certificate2(path);
                //var certhash = certificate.GetCertHashString();
                //Console.WriteLine(certhash);
                //var assembly = Assembly.GetExecutingAssembly();
                //var attribute = (GuidAttribute)assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0];
                //var appid = attribute.Value;
                //Console.WriteLine(appid);
                //netsh http add urlacl url=http://+:9875/ user=Everyone
                //netsh http add urlacl url=https://+:9876/ user=Everyone
                ////netsh http add sslcert ipport=0.0.0.0:9876 certhash=F8ACE24A36F93B006BFAF495F6C14FB827AC61A3 appid={00000000-0000-0000-0000-AABBCCDDEEFF}
                //Terminal.Terminal.Execute($"netsh http add urlacl url=http://+:{port}/ user=Everyone");
                //Terminal.Terminal.Execute($"netsh http add urlacl url=https://+:12344/ user=Everyone");
                //Terminal.Terminal.Execute($"netsh http add sslcert ipport=0.0.0.0:12344 certhash={certhash} appid={appid}");
            }
            catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
