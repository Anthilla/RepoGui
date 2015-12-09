using Nancy;
using Nancy.Conventions;

namespace aosrepo {
    public class Bootstrapper : DefaultNancyBootstrapper {
        protected override void ConfigureConventions(NancyConventions conv) {
            base.ConfigureConventions(conv);
            conv.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("Scripts", @"/Scripts/")
                );
            conv.StaticContentsConventions.Add(
                StaticContentConventionBuilder.AddDirectory("Fonts", @"/Fonts/")
                );
        }
    }
}