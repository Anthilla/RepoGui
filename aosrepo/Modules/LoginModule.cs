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
                var returnUrl = (string)Request.Query.returnUrl;
                model.ReturnUrl = returnUrl;
                model.Title = "Welcome" + returnUrl;
                model.Copyright = @"© 2013 - " + DateTime.Now.ToString("yyyy") + " Anthilla S.r.l.";
                return View["login", model];
            };

            Post["/login"] = x => {
                var cookies = Request.Cookies;
                cookies.Clear();
                cookies.Remove("aosrepo-session");
                var username = (string)Request.Form.Username;
                var password = (string)Request.Form.Password;
                var validationGuid = UserDatabase.ValidateUser(username, password);
                if (validationGuid == null) {
                    return Context.GetRedirect("/login");
                }
                var cookie = new NancyCookie("aosrepo-session", validationGuid.ToGuid().ToString());
                var returnUrl = (string)Request.Form.Return;
                return this.LoginAndRedirect(validationGuid.Value, DateTime.Now.AddDays(3), returnUrl).WithCookie(cookie);
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
