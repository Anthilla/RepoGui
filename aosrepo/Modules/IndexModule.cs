using System.Dynamic;
using System.IO;
using Nancy;
using System.Net.Http;

namespace aosrepo.Modules {
    public class IndexModule : NancyModule {

        private static string sharedRepoDirectory = "/Data/Dev01/AOS_Repo/repo.public";
        private static string serverListFile = "server.list";

        public IndexModule() {
            Get["/"] = x => {
                dynamic model = new ExpandoObject();
                model.Repo = Repository.GetAll();
                return View["home", model];
            };

            Get["/serverlist"] = x => {
                var serverListPath = $"{sharedRepoDirectory}/{serverListFile}";
                var list = File.ReadAllLines(serverListPath);
                return Response.AsJson(list);
            };

            Get["/filelist"] = x => {
                return Response.AsJson(Repository.GetFileInfo());
            };
        }
    }
}