using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using aosrepo.Model;

namespace aosrepo {
    public class Repository {
        private static string sharedRepoDirectory = "/Data/Dev01/AOS_Repo/repo.public";
        private static string serverListFile = "server.list";
        private static string repoListFile = "repo.list";

        public static IEnumerable<FileInfoModel> GetFileInfo() {
            try {
                var repoListPath = $"{sharedRepoDirectory}/{repoListFile}";
                var list = File.ReadAllLines(repoListPath);
                var files = new List<FileInfoModel>();
                foreach (var f in list) {
                    var s = f.Split(new[] { ' ' }, 3);
                    var fi = new FileInfoModel {
                        FileHash = s[0],
                        FileContext = s[1],
                        FileName = s[2]
                    };
                    files.Add(fi);
                }
                return files;
            }
            catch(Exception ex) {
                Console.WriteLine($"error: {ex.Message}");
                return new List<FileInfoModel>();
            }
        }

        public static string FileDirectory => "/cfg/aosrepo";

        public static IEnumerable<RepoModel> GetAll() {
            var info = GetFileInfo();
            if (info.Count() < 1)
                return new List<RepoModel>();
            try {
                var repos = new List<RepoModel>();
                var contexts = new HashSet<string>();
                foreach (var c in info) {
                    contexts.Add(c.FileContext);
                }
                foreach (var context in contexts) {
                    var list = new List<FileModel>();
                    var files = info.Where(_ => _.FileContext == context);
                    foreach (var file in files) {
                        var fpath = $"{sharedRepoDirectory}/{file.FileName}";
                        list.Add(new FileModel {
                            Guid = Guid.NewGuid().ToString(),
                            ShaSum = file.FileHash,
                            Date = GetDate(file.FileName).Trim(),
                            Order = GetOrder(file.FileName).Trim(),
                            FilePath = GetDownloadUrl(file.FileName),
                            FileName = file.FileName,
                            Size = GetSize(fpath).Trim(),
                            Device = "x86_64",
                            Type = "nightly",
                        });
                    }
                    repos.Add(new RepoModel {
                        Name = context,
                        Files = list.OrderByDescending(_ => _.Order).ThenByDescending(_ => _.FileName).ToList()
                    });
                }
                return repos;
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                return new List<RepoModel>();
            }
        }

        private static string GetDownloadUrl(string fileName) {
            var serverListPath = $"{sharedRepoDirectory}/{serverListFile}";
            var srv = File.ReadAllLines(serverListPath);
            return $"{srv.FirstOrDefault()}:8080/{fileName}";
        }

        private static string GetOrder(string path) {
            if (string.IsNullOrEmpty(path))
                return "";
            try {
                var fName = Path.GetFileName(path).Trim();
                var from = path.Contains("-aufs-")
                    ? fName.IndexOf("-aufs-", StringComparison.InvariantCulture) + "-aufs-".Length
                    : fName.IndexOf("-", StringComparison.InvariantCulture) + "-".Length;
                var to = fName.LastIndexOf(path.Contains("-x86_64") ? "-x86_6" : ".squashfs.xz", StringComparison.InvariantCulture);
                return fName.Substring(from, to - from);
            }
            catch (Exception) {
                return "";
            }
        }

        private static string GetDate(string path) {
            if (string.IsNullOrEmpty(path))
                return "";
            try {
                var date = GetOrder(path);
                var myDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.InvariantCulture);
                return myDate.ToString("yyyy MMMM dd");
            }
            catch (Exception) {
                return "";
            }
        }

        private static string GetSize(string path) {
            if (!File.Exists(path))
                return "";
            try {
                var value = Terminal.Terminal.Execute($"stat -c '%s' {path}");
                var ivalue = Convert.ToInt64(value);
                var mega = ivalue / 1024f / 1024f;
                return mega.ToString("0.00") + " MB";
            }
            catch (Exception) {
                return "";
            }
        }

        private static string GetShaSum(string path) {
            return !File.Exists(path) ? null : Terminal.Terminal.Execute($"sha1sum {path}").Split(' ').First();
        }

        private static string GetLastFile() {
            try {
                var files = Directory.EnumerateFiles(FileDirectory, "*.json").ToList();
                return !files.Any() ? null : files.Last();
            }
            catch (Exception) {
                return null;
            }
        }

        public static RepoModel GetByName(string name) {
            return GetAll().FirstOrDefault(_ => _.Name == name);
        }

        public static string GetFilePath(string guid) {
            var list = new List<FileModel>();
            foreach (var files in GetAll()) {
                list.AddRange(files.Files);
            }
            return list.First(_ => _.Guid == guid).FilePath;
        }
    }
}
