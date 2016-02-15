using System.Collections.Generic;
using System.IO;

namespace aosrepo {
    public class Settings {
        public static void Update(string text) {
            var path = "/cfg/aosrepo/config/watch.cfg";
            File.WriteAllText(path, text);
        }

        public static IEnumerable<string> GetDirectories() {
            var path = "/cfg/aosrepo/config/watch.cfg";
            return File.ReadAllLines(path);
        }

        public static string GetDirectoriesAsText() {
            var path = "/cfg/aosrepo/config/watch.cfg";
            return File.ReadAllText(path);
        }
    }
}
