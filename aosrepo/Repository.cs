using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using aosrepo.Model;

namespace aosrepo {
    public class Repository {
        private const string RepoListFilePath = "/Data/Dev01/AOS_Repo/repo.public/repo.txt.bz2";
        private const string ReadableRepoListFilePath = "/Data/Dev01/AOS_Repo/repo.public/repo.txt";

        public Repository() {
            if (File.Exists(ReadableRepoListFilePath)) {
                File.Delete(ReadableRepoListFilePath);
            }
            Terminal.Terminal.Execute($"bunzip2 -k {RepoListFilePath}");
        }

        public IEnumerable<RepoModel> GetAll() {
            var info = GetFileInfo().ToList();
            if (!info.Any())
                return new List<RepoModel>();
            try {
                var repos = new List<RepoModel>();
                var contexts = new HashSet<string>();
                foreach (var c in info) {
                    contexts.Add(c.FileContext);
                }
                foreach (var context in contexts) {
                    var list = new List<FileModel>();
                    foreach (var file in info.Where(_ => _.FileContext == context)) {
                        var filepath = $"/Data/Dev01/AOS_Repo/repo.public/{file.FileName}";
                        list.Add(new FileModel {
                            Guid = Guid.NewGuid().ToString(),
                            ShaSum = file.FileHash,
                            Date = GetDate(file.FileName).Trim(),
                            Order = GetOrder(file.FileName).Trim(),
                            FilePath = $"http://srv.anthilla.com/{file.FileName}",
                            FileName = file.FileName,
                            Size = GetSize(filepath).Trim(),
                            Device = "x86_64",
                            Type = "nightly"
                        });
                    }
                    repos.Add(new RepoModel {
                        Name = context,
                        Files = list.OrderByDescending(_ => _.Order).ThenByDescending(_ => _.FileName).ToList()
                    });
                }
                return repos;
            }
            catch (Exception) {
                return new List<RepoModel>();
            }
        }

        private static IEnumerable<FileInfoModel> GetFileInfo() {
            try {
                var list = File.ReadAllLines(ReadableRepoListFilePath);
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
            catch (Exception) {
                return new List<FileInfoModel>();
            }
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
    }
}
