using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace aosrepo {
    public class OldRepository {
        public static void Update() {
            Console.WriteLine("doing update");
            var dirs = new List<string> {
                "/Data/Dev01/AOS_Repo/update.antd",
                "/Data/Dev01/AOS_Repo/update.antdsh",
                "/Data/Dev01/AOS_Repo/update.kernel",
                "/Data/Dev01/AOS_Repo/update.system"
            };
            ListDirectories(dirs, Settings.GetDirectories());
        }

        public static string FileDirectory => "/cfg/aosrepo";

        private static void ListDirectories(IEnumerable<string> directories) {
            if (!Directory.Exists(FileDirectory))
                return;
            try {
                var repos = new List<RepoModel>();
                foreach (var directory in directories.Where(_ => Directory.Exists(_))) {
                    var list = new List<FileModel>();
                    var files = Directory.EnumerateFiles(directory).Where(_ =>
                    _.EndsWith(".squashfs.xz") ||
                    _.Contains("System.map") ||
                    _.Contains("initramfs") ||
                    _.Contains("kernel")
                    ).ToList();
                    foreach (var file in files) {
                        list.Add(new FileModel {
                            Guid = Guid.NewGuid().ToString(),
                            ShaSum = GetShaSum(file).Trim(),
                            Date = GetDate(file).Trim(),
                            Order = GetOrder(file).Trim(),
                            FilePath = file.Trim().TrimStart('/'),
                            FileName = Path.GetFileName(file).Trim(),
                            Size = GetSize(file).Trim(),
                            Device = "x86_64",
                            Type = "nightly",
                        });
                    }
                    repos.Add(new RepoModel {
                        Name = directory.Split('/').Last().Split('.').Last().ToLower(),
                        Files = list.OrderByDescending(_ => _.Order).ThenByDescending(_ => _.FileName).ToList()
                    });
                }
                var filePath = $"{FileDirectory}/{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-filerepo.json";
                var json = JsonConvert.SerializeObject(repos);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
            }
        }

        private static void ListDirectories(IEnumerable<string> directories, IEnumerable<string> otherDirectories) {
            if (!Directory.Exists(FileDirectory))
                return;
            try {
                var repos = new List<RepoModel>();
                foreach (var directory in directories.Where(_ => Directory.Exists(_))) {
                    var list = new List<FileModel>();
                    var files = Directory.EnumerateFiles(directory).Where(_ =>
                    _.EndsWith(".squashfs.xz") ||
                    _.Contains("System.map") ||
                    _.Contains("initramfs") ||
                    _.Contains("kernel")
                    ).ToList();
                    foreach (var file in files) {
                        list.Add(new FileModel {
                            Guid = Guid.NewGuid().ToString(),
                            ShaSum = GetShaSum(file).Trim(),
                            Date = GetDate(file).Trim(),
                            Order = GetOrder(file).Trim(),
                            FilePath = file.Trim().TrimStart('/'),
                            FileName = Path.GetFileName(file).Trim(),
                            Size = GetSize(file).Trim(),
                            Device = "x86_64",
                            Type = "nightly",
                        });
                    }
                    repos.Add(new RepoModel {
                        Name = directory.Split('/').Last().Split('.').Last().ToLower(),
                        Files = list.OrderByDescending(_ => _.Order).ThenByDescending(_ => _.FileName).ToList()
                    });
                }
                foreach (var directory in otherDirectories.Where(_ => Directory.Exists(_))) {
                    var list = new List<FileModel>();
                    var files = Directory.EnumerateFiles(directory).ToList();
                    foreach (var file in files) {
                        list.Add(new FileModel {
                            Guid = Guid.NewGuid().ToString(),
                            ShaSum = GetShaSum(file).Trim(),
                            Date = GetDate(file).Trim(),
                            Order = GetOrder(file).Trim(),
                            FilePath = file.Trim().TrimStart('/'),
                            FileName = Path.GetFileName(file).Trim(),
                            Size = GetSize(file).Trim(),
                            Device = "x86_64",
                            Type = "nightly",
                        });
                    }
                    repos.Add(new RepoModel {
                        Name = directory.Split('/').Last().Split('.').Last().ToLower(),
                        Files = list.OrderByDescending(_ => _.Order).ThenByDescending(_ => _.FileName).ToList()
                    });
                }
                var filePath = $"{FileDirectory}/{DateTime.Now.ToString("yyyyMMddHHmmssfff")}-filerepo.json";
                var json = JsonConvert.SerializeObject(repos);
                File.WriteAllText(filePath, json);
            }
            catch (Exception ex) {
                Console.WriteLine(ex);
                throw;
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

        public static IEnumerable<RepoModel> GetAll() {
            var repoFile = GetLastFile();
            if (!File.Exists(repoFile) || string.IsNullOrEmpty(repoFile))
                return new List<RepoModel>();
            var text = File.ReadAllText(repoFile);
            try {
                return text.Length < 1
                    ? new List<RepoModel>()
                    : JsonConvert.DeserializeObject<IEnumerable<RepoModel>>(text);
            }
            catch (Exception) {
                return new List<RepoModel>();
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

        public static void LogDownload(string fileName, string requestSource) {
            if (!Directory.Exists(FileDirectory))
                return;
            var filePath = $"{FileDirectory}/download-log.txt";
            if (!File.Exists(filePath)) {
                File.WriteAllText(filePath, "Anthilla Repository - Download Log");
            }
            File.AppendAllLines(filePath, new[] { $"{DateTime.Now.ToString("yyyyMMdd")} - {fileName} from {requestSource}" });
        }
    }
}
