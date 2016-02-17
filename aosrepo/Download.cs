using Nancy;
using Nancy.Responses;
using System.IO;

namespace aosrepo {
    public class FileManagement {
        public static Response DownloadAsResponse(string path, string downloadName) {
            var response = new Response();
            response.Headers.Add("Content-Disposition", "attachment; filename=" + downloadName);
            response.ContentType = "text/plain";
            response.Contents = stream => {
                using (var fileStream = File.OpenRead($"/{path}")) {
                    using (var memoryStream = new MemoryStream()) {
                        fileStream.CopyTo(memoryStream);
                        int data;
                        while ((data = memoryStream.ReadByte()) != -1) {
                            memoryStream.WriteByte((byte)data);
                        }
                    }
                }
            };
            return response;
        }

        public static StreamResponse DownloadAsStreamResponse(string path, string downloadName) {
            var file = new FileStream($"/{path}", FileMode.Open);
            return new StreamResponse(() => file, MimeTypes.GetMimeType(downloadName));
        }
    }
}
