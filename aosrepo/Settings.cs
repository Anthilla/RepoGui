using System.Collections.Generic;
using System.IO;

namespace aosrepo {
    public class Settings {
        public static void Update(IEnumerable<string> text) {
            var path = "/cfg/aosrepo/config/watch.cfg";
            File.WriteAllLines(path, text);
        }

        public static IEnumerable<string> GetDirectories() {
            var path = "/cfg/aosrepo/config/watch.cfg";
            return File.ReadAllLines(path);
        }
    }
}
