using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Nancy;
using Nancy.Responses;

namespace aosrepo {
    public class IndexModule : NancyModule {
        public IndexModule() {

            Get["/"] = x => {
                dynamic model = new ExpandoObject();
                model.Repo = Repository.GetAll();
                return View["home", model];
            };

            Get["/update"] = x => Update();
            Post["/update"] = x => Update();

            Get["/download/{guid}/{file}"] = x => {
                var guid = (string)x.guid;
                var fileName = (string)x.file;
                var path = Repository.GetFilePath(guid);
                var file = new FileStream($"/{path}", FileMode.Open);
                var response = new StreamResponse(() => file, MimeTypes.GetMimeType(fileName));
                return response.AsAttachment(fileName);
            };

            Get["/update/context"] = x => {
                var list = new List<string> { "antd", "antdsh", "kernel", "system" };
                return Response.AsJson(list);
            };

            Get["/update/info/{item}/{date}"] = x => {
                var item = (string)x.item;
                var date = (string)x.date;
                var response = UpdateManagement.GetUpdateInfo(item, date);
                return Response.AsJson(response);
            };
        }

        private static HttpStatusCode Update() {
            Repository.Update();
            return HttpStatusCode.OK;
        }
    }
}