using Owin;
using System;
using System.IO;
using Microsoft.Owin.Hosting;
using System.Net;

namespace aosrepo {
    internal static class Program {
        private static void Main() {
            Console.Title = "aosrepo";
            ServerConfiguration.CheckDirectories();
            var ip = ServerConfiguration.GetServerIp();
            var port = ServerConfiguration.GetServerPort();
            ServerConfiguration.SetMasterFile();
            ServerConfiguration.SetSettingsFile();
            using (WebApp.Start<Startup>($"http://{ip}:{port}/")) {
                Console.WriteLine($"Running on http://{ip}:{port}/");
                KeepAlive();
            }
        }

        private static void KeepAlive() {
            var r = Console.ReadLine();
            while (r != "quit") {
                r = Console.ReadLine();
            }
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            object httpListener;
            if (app.Properties.TryGetValue(typeof(HttpListener).FullName, out httpListener) && httpListener is HttpListener) {
                ((HttpListener)httpListener).IgnoreWriteExceptions = true;
            }
            //StaticConfiguration.DisableErrorTraces = false;
            app.UseNancy();
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

        public static string SetMasterFile() {
            const string configDir = "/cfg/aosrepo/config";
            var portConfigFile = $"{configDir}/master.cfg";
            if (!File.Exists(portConfigFile)) {
                File.WriteAllText(portConfigFile, "user:password");
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
