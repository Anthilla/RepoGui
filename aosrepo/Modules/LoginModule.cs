using aosrepo.Login;
using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Cookies;
using Nancy.Extensions;
using System;
using System.Dynamic;

namespace aosrepo.Modules {
    public class LoginModule : NancyModule {
        public LoginModule() {
            Get["/login"] = x => {
                dynamic model = new ExpandoObject();
                model.Title = "Welcome";
                model.Copyright = @"© 2013 - " + DateTime.Now.ToString("yyyy") + " Anthilla S.r.l.";
                return View["login", model];
            };

            Post["/login"] = x => {
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                var validationGuid = UserDatabase.ValidateUser(username, password);
                if (validationGuid == null) {
                    return Context.GetRedirect("/login");
                }
                var cookies = Request.Cookies;
                cookies.Clear();
                cookies.Remove("aosrepo-session");
                var cookie = new NancyCookie("aosrepo-session", validationGuid.ToGuid().ToString());
                return this.LoginAndRedirect(validationGuid.Value, DateTime.Now.AddDays(3)).WithCookie(cookie);
            };

            Get["/logout"] = x => {
                var request = Request;
                var cookies = request.Cookies;
                cookies.Clear();
                return this.LogoutAndRedirect("/");
            };
        }
    }
}
