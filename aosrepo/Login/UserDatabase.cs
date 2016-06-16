using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.IO;

namespace aosrepo.Login {
    public class UserDatabase : IUserMapper {
        private static Guid _guid = Guid.Parse("ACE0B402-87D1-45A0-9DA6-7B6C10B9B894");

        private static string MasterPassword
        {
            get
            {
                const string path = "/cfg/aosrepo/config/master.cfg";
                return !File.Exists(path) ? string.Empty : File.ReadAllText(path.Trim());
            }
        }

        private static UserIdentity Master
        {
            get
            {
                return new UserIdentity {
                    Id = _guid,
                    UserName = "master",
                    Claims = new List<string> { "master", _guid.ToString(), MasterPassword }
                };
            }
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context) {
            return identifier == _guid ? Master : null;
        }

        public Guid? ValidateUser(string userIdentity, string password) {
            if (string.Equals(password, MasterPassword, StringComparison.CurrentCultureIgnoreCase)) {
                return _guid;
            }
            return null;
        }
    }
}