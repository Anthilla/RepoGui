using ICSharpCode.SharpZipLib.GZip;
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace aosrepo {
    public static class RequestHandling {
        public static Func<NancyContext, string, Response> AddDirectoryWithExpiresHeader(string requestedPath, string contentPath, TimeSpan expiresTimeSpan, params string[] allowedExtensions) {
            var responseBuilder = StaticContentConventionBuilder.AddDirectory(requestedPath, contentPath, allowedExtensions);
            return (context, root) => {
                var response = responseBuilder(context, root);
                if (response != null) {
                    response.Headers.Add("Expires", DateTime.Now.Add(expiresTimeSpan).ToString("R"));
                    response.Headers["Transfer-Encoding"] = "chunked";
                    response.Headers["Content-Encoding"] = "gzip";
                    response.Headers["Cache-Control"] = "private, max-age=0";
                    var contents = response.Contents;
                    response.Contents = responseStream => {
                        using (var compression = new GZipOutputStream(responseStream)) {
                            compression.SetLevel(9);
                            contents(compression);
                        }
                    };
                }
                return response;
            };
        }

        public static void RegisterCompressionCheck(this IPipelines pipelines) {
            pipelines.AfterRequest.AddItemToEndOfPipeline(CheckForCompression);
        }

        static void CheckForCompression(NancyContext context) {
            if (!RequestIsGzipCompatible(context.Request)) {
                return;
            }

            if (context.Response.StatusCode != HttpStatusCode.OK) {
                return;
            }

            if (!ResponseIsCompatibleMimeType(context.Response)) {
                return;
            }

            if (ContentLengthIsTooSmall(context.Response)) {
                return;
            }

            CompressResponse(context.Response);
        }

        static void CompressResponse(Response response) {
            response.Headers.Add("Expires", DateTime.Now.Add(TimeSpan.FromDays(365)).ToString("R"));
            response.Headers["Transfer-Encoding"] = "chunked";
            response.Headers["Content-Encoding"] = "gzip";
            response.Headers["Cache-Control"] = "private, max-age=0";
            response.ContentType = "text/html; charset=utf-8";
            var contents = response.Contents;
            response.Contents = responseStream => {
                using (var compression = new GZipOutputStream(responseStream)) {
                    compression.SetLevel(9);
                    contents(compression);
                }
            };
        }

        static bool ContentLengthIsTooSmall(Response response) {
            string contentLength;
            if (response.Headers.TryGetValue("Content-Length", out contentLength)) {
                var length = long.Parse(contentLength);
                if (length < 4096) {
                    return true;
                }
            }
            return false;
        }

        public static List<string> ValidMimes = new List<string> {
                                                    "text/css",
                                                    "text/html",
                                                    "text/plain",
                                                    "image/png",
                                                    "application/xml",
                                                    "application/json",
                                                    "application/xaml+xml",
                                                    "application/x-javascript",
                                                    "application/javascript"
                                                };

        static bool ResponseIsCompatibleMimeType(Response response) {
            return ValidMimes.Any(x => x == response.ContentType);
        }

        static bool RequestIsGzipCompatible(Request request) {
            return request.Headers.AcceptEncoding.Any(x => x.Contains("gzip"));
        }
    }
}
