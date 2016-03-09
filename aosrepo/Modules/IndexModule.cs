using System.Dynamic;
using Nancy;

namespace aosrepo.Modules {
    public class IndexModule : NancyModule {
        public IndexModule() {
            Get["/"] = x => {
                dynamic model = new ExpandoObject();
                model.Repo = new Repository().GetAll();
                return View["home", model];
            };
        }
    }
}