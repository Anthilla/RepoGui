using System;
using System.Collections.Generic;
using Nancy.Security;

namespace aosrepo.Login {
    public class UserIdentity : IUserIdentity {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }
}