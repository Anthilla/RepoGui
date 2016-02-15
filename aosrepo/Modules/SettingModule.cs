using Nancy;
using Nancy.Security;
using System.Dynamic;
using System;

namespace aosrepo.Modules {
    public class SettingModule : NancyModule {
        public SettingModule() {
            this.RequiresAuthentication();

            Get["/settings"] = x => {
                dynamic model = new ExpandoObject();
                model.Directories = Settings.GetDirectories();
                return View["settings", model];
            };

            Post["/settings"] = x => {
                var text = (string)Request.Form.Text;
                Settings.Update(text.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries));
                return Response.AsRedirect("/settings");
            };
        }
    }
}
