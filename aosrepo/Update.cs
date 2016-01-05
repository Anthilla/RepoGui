using System;
using System.Globalization;
using System.Linq;

namespace aosrepo {
    public class UpdateManagement {
        public static Tuple<bool, string, string, string> GetUpdateInfo(string context, string currentVersion) {
            Repository.Update();
            var repo = Repository.GetByName(context);
            var newestFile = repo.Files.First();
            var currentDate = DateTime.ParseExact(currentVersion, "yyyyMMdd", CultureInfo.InvariantCulture);
            var date = DateTime.ParseExact(newestFile.Order, "yyyyMMdd", CultureInfo.InvariantCulture);
            var isUptodate = date > currentDate;
            return new Tuple<bool, string, string, string>(isUptodate, newestFile.Guid, newestFile.FileName, newestFile.ShaSum);
        }
    }
}
