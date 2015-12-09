using System;
using System.IO;
using System.Threading;
using Microsoft.Owin.Hosting;
using Nancy;
using Owin;

namespace aosrepo {
    internal static class Program {
        private static void Main() {
            Console.Title = "aosrepo";
            var stop = new ManualResetEvent(false);
            Console.CancelKeyPress +=
                (sender, e) => {
                    Console.WriteLine("^C");
                    Environment.Exit(1);
                    stop.Set();
                    e.Cancel = true;
                };

            using (WebApp.Start<Startup>("http://+:80/")) {
                Console.WriteLine("application listening on http://+:80/");
                Directory.CreateDirectory(Repository.FileDirectory);
                stop.WaitOne();
            }
        }
    }

    internal class Startup {
        public void Configuration(IAppBuilder app) {
            StaticConfiguration.DisableErrorTraces = false;
            app.UseNancy();
        }
    }
}
