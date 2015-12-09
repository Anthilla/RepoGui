
using System.Collections.Generic;

namespace aosrepo {
    public class RepoModel {
        public string Name { get; set; }
        public List<FileModel> Files { get; set; }
    }

    public class FileModel {
        public string Guid { get; set; }
        public string ShaSum { get; set; }
        public string Date { get; set; }
        public string Order { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string Size { get; set; }
        public string Device { get; set; }
        public string Type { get; set; }
    }
}
