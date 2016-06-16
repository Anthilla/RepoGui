using System.Collections.Generic;
using System.IO;

namespace aosrepo {
    public class Settings {
        public static void Update(string text) {
            const string path = "/cfg/aosrepo/config/watch.cfg";
            File.WriteAllText(path, text);
        }

        public static IEnumerable<string> GetDirectories() {
            const string path = "/cfg/aosrepo/config/watch.cfg";
            return File.ReadAllLines(path);
        }

        public static string GetDirectoriesAsText() {
            const string path = "/cfg/aosrepo/config/watch.cfg";
            return File.ReadAllText(path);
        }
    }
}
