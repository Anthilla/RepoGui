using aosrepo.Middleware;
using Nancy.Owin;
using Owin;
using System;
using System.IO;
using System.Threading;
using Microsoft.Owin.Hosting;
using System.Net;

namespace aosrepo {
    internal static class Program {
        private static void Main() {
            try {
                Console.Title = "aosrepo";
                ServerConfiguration.CheckDirectories();
                var ip = ServerConfiguration.GetServerIp();
                var port = ServerConfiguration.GetServerPort();
                ServerConfiguration.SetSettingsFile();
                using (WebApp.Start<Startup>($"http://{ip}:{port}/")) {
                    Console.WriteLine($"Running on http://{ip}:{port}/");
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
            object httpListener;
            if (app.Properties.TryGetValue(typeof(HttpListener).FullName, out httpListener) && httpListener is HttpListener) {
                ((HttpListener)httpListener).IgnoreWriteExceptions = true;
            }
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

        public static void SetSettingsFile() {
            const string configDir = "/cfg/aosrepo/config";
            var portConfigFile = $"{configDir}/watch.cfg";
            if (!File.Exists(portConfigFile)) {
                File.WriteAllText(portConfigFile, "");
            }
        }
    }
}
