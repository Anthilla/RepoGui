using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using Nancy;
using Nancy.Responses;
using System;

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

            Get["/old/download/{guid}/{file}"] = x => {
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

            Get["/old/update/context"] = x => {
                var list = new List<string> { "antd", "antdsh", "kernel", "system" };
                return Response.AsJson(list);
            };

            Get["/old/update/info/{item}/{date}"] = x => {
                var item = (string)x.item;
                var date = (string)x.date;
                var response = UpdateManagement.GetUpdateInfo(item, date);
                return Response.AsJson(response);
            };
        }
    }
}