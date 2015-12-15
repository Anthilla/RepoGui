using aosrepo.Middleware;
using Nancy.Owin;
using Owin;
using System;
using System.IO;
using System.Threading;
using Microsoft.Owin.Hosting;
using System.Net;
//using Microsoft.Owin.Builder;
//using Nowin;
//using System.Threading.Tasks;
//using System.Collections.Generic;
//using System.Security.Cryptography.X509Certificates;
//using Owin.RequiresHttps;

namespace aosrepo {
    internal static class Program {
        static void Main(string[] args) {
            try {
                ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
                Console.Title = "aosrepo";
                ServerConfiguration.CheckDirectories();
                var ip = ServerConfiguration.GetServerIp();
                var port = ServerConfiguration.GetServerPort();
                using (WebApp.Start<Startup>($"http://{ip}:{port}/")) {
                    Console.WriteLine($"Running a http server on http://{ip}:{port}/");
                    do {
                        Thread.Sleep(60000);
                    } while (!Console.KeyAvailable);
                }

                //var appbuilder = new AppBuilder();
                //new Startup().Configuration(appbuilder);

                //var httpsPort = Convert.ToInt32(port);
                //var httpsEndPoint = new IPEndPoint(IPAddress.Parse(ip), httpsPort);
                //var httpsBuilder = ServerBuilder.New()
                //    .SetEndPoint(httpsEndPoint)
                //    .SetOwinApp(appbuilder.Build())
                //    //.SetOwinCapabilities((IDictionary<string, object>)appbuilder.Properties[OwinKeys.ServerCapabilitiesKey])
                //    .SetExecutionContextFlow(ExecutionContextFlow.SuppressAlways)
                //    //.SetCertificate(new X509Certificate2("/cfg/aosrepo/certificate.pfx"))
                //    //.RequireClientCertificate();

                //using (var httpsServer = httpsBuilder.Build()) {
                //    Task.Run(() => httpsServer.Start());
                //    Console.WriteLine($"Running a http server on http://{ip}:{port}/");
                //    do {
                //        Thread.Sleep(60000);
                //    } while (!Console.KeyAvailable);
                //}
            }
            catch (Exception ex) {
                const string dir = "/cfg/aosrepo";
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                var reportDir = $"{dir}/report";
                if (!Directory.Exists(reportDir)) {
                    Directory.CreateDirectory(reportDir);
                }
                var file = $"{reportDir}/{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-crash-report.txt";
                File.WriteAllText(file, ex.ToString());
            }
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            //app.UseCertificate("/cfg/aosrepo/certificate.pfx");

            //var redirectOptions = new RequiresHttpsOptions() { RedirectToHttpsPath = "https://127.0.0.1:12344/" };
            //app.RequiresHttps(redirectOptions);

            app.UseDebugMiddleware();
            app.UseNancy();
            app.UseDebugMiddleware(new DebugMiddlewareOptions() {
                OnIncomingRequest = context => context.Response.WriteAsync("## Beginning ##"),
                OnOutGoingRequest = context => context.Response.WriteAsync("## End ##")
            });
            app.UseNancy(options => options.PassThroughWhenStatusCodesAre(Nancy.HttpStatusCode.NotFound));
        }
    }

    internal class ServerConfiguration {
        public static void CheckDirectories() {
            const string dir = "/cfg/aosrepo";
            if (!Directory.Exists(dir)) {
                Directory.CreateDirectory(dir);
            }
            var configDir = $"{dir}/config";
            if (!Directory.Exists(configDir)) {
                Directory.CreateDirectory(configDir);
            }
        }

        public static string GetServerPort() {
            const string configDir = "/cfg/aosrepo/config";
            var portConfigFile = $"{configDir}/port.cfg";
            if (!File.Exists(portConfigFile)) {
                File.WriteAllText(portConfigFile, "80");
            }
            return File.ReadAllText(portConfigFile).Trim();
        }

        public static string GetServerIp() {
            const string configDir = "/cfg/aosrepo/config";
            var portConfigFile = $"{configDir}/ip.cfg";
            if (!File.Exists(portConfigFile)) {
                File.WriteAllText(portConfigFile, "+");
            }
            return File.ReadAllText(portConfigFile).Trim();
        }
    }
}
