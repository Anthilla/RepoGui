using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using Nancy;
using Nancy.Responses;
using System;

namespace aosrepo.Modules {
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
                try {
                    var fileInfo = new FileInfo(path);
                    var response = new Response();
                    response.ContentType = MimeTypes.GetMimeType(path);
                    response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                    response.Contents = stream => {
                        using (var fileStream = File.OpenRead($"/{path}")) {
                            fileStream.CopyTo(stream);
                        }
                    };
                    return response;
                }
                catch (Exception ex) {
                    Console.WriteLine($"download failed: {ex}");
                    Console.WriteLine("retry file download");
                    var file = new FileStream($"/{path}", FileMode.Open);
                    var response = new StreamResponse(() => file, MimeTypes.GetMimeType(path));
                    return response.AsAttachment(fileName);
                }
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

            Get["/update/antd"] = x => {
                var repo = Repository.GetByName("antd");
                var latestFile = repo.Files.OrderBy(_ => _.Date).LastOrDefault();
                var guid = latestFile.Guid;
                var fileName = latestFile.FileName;
                var path = Repository.GetFilePath(guid);
                try {
                    var fileInfo = new FileInfo(path);
                    var response = new Response();
                    response.ContentType = MimeTypes.GetMimeType(path);
                    response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    response.Headers.Add("Content-Length", fileInfo.Length.ToString());
                    response.Contents = stream => {
                        using (var fileStream = File.OpenRead($"/{path}")) {
                            fileStream.CopyTo(stream);
                        }
                    };
                    return response;
                }
                catch (Exception ex) {
                    Console.WriteLine($"download failed: {ex}");
                    Console.WriteLine("retry file download");
                    var file = new FileStream($"/{path}", FileMode.Open);
                    var response = new StreamResponse(() => file, MimeTypes.GetMimeType(path));
                    return response.AsAttachment(fileName);
                }
            };
        }

        private static HttpStatusCode Update() {
            Repository.Update();
            Console.WriteLine("Repository update...");
            return HttpStatusCode.OK;
        }
    }
}