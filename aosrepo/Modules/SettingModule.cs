using Nancy;
using Nancy.Security;
using System.Dynamic;

namespace aosrepo.Modules {
    public class SettingModule : NancyModule {
        public SettingModule() {
            this.RequiresAuthentication();

            Get["/settings"] = x => {
                dynamic model = new ExpandoObject();
                model.Directories = Settings.GetDirectoriesAsText();
                return View["settings", model];
            };

            Post["/settings"] = x => {
                var text = (string)Request.Form.Text;
                Settings.Update(text);
                return Response.AsRedirect("/settings");
            };
        }
    }
}
