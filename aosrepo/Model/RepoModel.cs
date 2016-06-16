using System.Collections.Generic;

namespace aosrepo.Model {
    public class RepoModel {
        public string Name { get; set; }
        public IEnumerable<FileModel> Files { get; set; }
    }
}
