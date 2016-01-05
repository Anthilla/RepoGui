using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace aosrepo {
    public class UpdateManagement {
        public static IEnumerable<KeyValuePair<string, string>> GetUpdateInfo(string context, string currentVersion) {
            Repository.Update();
            var repo = Repository.GetByName(context);
            var newestFile = repo.Files.First();
            var currentDate = DateTime.ParseExact(currentVersion, "yyyyMMdd", CultureInfo.InvariantCulture);
            var date = DateTime.ParseExact(newestFile.Order, "yyyyMMdd", CultureInfo.InvariantCulture);
            return new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("request-context", context),
                new KeyValuePair<string, string>("request-version", currentVersion),
                new KeyValuePair<string, string>("version", newestFile.Order),
                new KeyValuePair<string, string>("isuptodate", (date > currentDate).ToString()),
                new KeyValuePair<string, string>("guid", newestFile.Guid),
                new KeyValuePair<string, string>("hash", newestFile.ShaSum),
                new KeyValuePair<string, string>("type", newestFile.Type),
                new KeyValuePair<string, string>("url", $"/download/{newestFile.Guid}/{newestFile.FileName}"),
            };
        }
    }
}
