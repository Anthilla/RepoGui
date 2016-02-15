using System;

namespace aosrepo {
    public static class Extensions {
        public static Guid ToGuid(this Guid? source) {
            return source ?? Guid.Empty;
        }
    }
}
