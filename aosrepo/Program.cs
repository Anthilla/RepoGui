using System;
using System.IO;
using System.Linq;
using Microsoft.Owin.Hosting;
using Owin;
using Nancy;
using System.Threading;
using Nancy.Owin;
using aosrepo.Middleware;

namespace aosrepo {
    internal static class Program {
        static void Main(string[] args) {
            try {
                Console.Title = "aosrepo";
                ServerConfiguration.CheckDirectories();
                var port = ServerConfiguration.GetServerPort();
                var ip = ServerConfiguration.GetServerIp();
                var options = new StartOptions();
                var urls = new[] { $"http://{ip}:{port}/" };
                urls.ToList().ForEach(options.Urls.Add);
                using (WebApp.Start<Startup>(options)) {
                    Console.WriteLine("Running a http server on {0}", string.Join(", ", options.Urls));
                    do {
                        Thread.Sleep(60000);
                    } while (!Console.KeyAvailable);
                }
            }
            catch (Exception ex) {
                const string dir = "/cfg/aosrepo";
                if (!Directory.Exists(dir)) {
                    Directory.CreateDirectory(dir);
                }
                var file = $"{dir}/{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-crash-report.txt";
                File.WriteAllText(file, ex.ToString());
            }
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            //StaticConfiguration.DisableErrorTraces = false;
            app.UseDebugMiddleware();
            app.UseNancy();
            app.UseDebugMiddleware(new DebugMiddlewareOptions() {
                OnIncomingRequest = context => context.Response.WriteAsync("## Beginning ##"),
                OnOutGoingRequest = context => context.Response.WriteAsync("## End ##")
            });
            //app.Map("/nancy", mappedApplication => mappedApplication.UseNancy());
            app.UseNancy(options => options.PassThroughWhenStatusCodesAre(HttpStatusCode.NotFound));
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
