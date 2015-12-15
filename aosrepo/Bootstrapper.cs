using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using Nancy.TinyIoc;
using System;

namespace aosrepo {
    public class Bootstrapper : DefaultNancyBootstrapper {
        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Clear();
            conv.StaticContentsConventions.Add(RequestHandling.AddDirectoryWithExpiresHeader("Content", @"/Content/", TimeSpan.FromDays(365)));
            conv.StaticContentsConventions.Add(RequestHandling.AddDirectoryWithExpiresHeader("Scripts", @"/Scripts/", TimeSpan.FromDays(365)));
            conv.StaticContentsConventions.Add(RequestHandling.AddDirectoryWithExpiresHeader("Fonts", @"/Fonts/", TimeSpan.FromDays(365)));
        }

        protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines) {
            base.ApplicationStartup(container, pipelines);
            pipelines.RegisterCompressionCheck();
        }

        protected override void RequestStartup(TinyIoCContainer container, IPipelines pipelines, NancyContext context) {
            base.RequestStartup(container, pipelines, context);
        }
    }
}