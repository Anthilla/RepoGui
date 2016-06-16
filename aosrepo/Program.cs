using Owin;
using System;
using System.IO;
using Microsoft.Owin.Hosting;
using System.Net;

namespace aosrepo {
    internal static class Program {
        private static void Main(string[] args) {
            try {
                Console.Title = "aosrepo";
                ServerConfiguration.CheckDirectories();
                var ip = ServerConfiguration.GetServerIp();
                var port = ServerConfiguration.GetServerPort();
                ServerConfiguration.SetMasterFile();
                ServerConfiguration.SetSettingsFile();
                using (WebApp.Start<Startup>($"http://{ip}:{port}/")) {
                    Console.WriteLine($"Running on http://{ip}:{port}/");
                    while (true) {
                        var command = args.Length > 0 ? args[0] : Console.ReadLine();
                        if (!string.IsNullOrEmpty(command)) {
                            Command.Loop(command.Trim());
                        }
                    }
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

    public class Command {
        public static void Loop(string command) {
            switch (command) {
                case "update":
                    Console.WriteLine("Repository up to date!");
                    return;
                default:
                    Console.WriteLine($"{command} command does not exist");
                    return;
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
                File.WriteAllText(portConfigFile, "change your password!!");
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
