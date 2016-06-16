using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace aosrepo.Login {

    public class UserIdentity : IUserIdentity {
        public string UserName { get; set; }
        public IEnumerable<string> Claims { get; set; }
    }

    public class UserDatabase : IUserMapper {
        private static IEnumerable<UserEntity.UserEntityModel> Users() {
            var guid = Guid.Parse("ACE0B402-87D1-45A0-9DA6-7B6C10B9B894");
            var userList = new List<UserEntity.UserEntityModel> {
                new UserEntity.UserEntityModel {
                    Id = guid,
                    MasterGuid = guid.ToString(),
                    MasterUsername = "master",
                    IsEnabled = true,
                    Claims = new List<UserEntity.UserEntityModel.Claim> {
                        new UserEntity.UserEntityModel.Claim {
                            ClaimGuid = guid.ToString(),
                            Type= UserEntity.ClaimType.UserPassword,
                            Key = "master-password",
                            Value= GetMasterPassword()
                        }
                    }
                }
            };
            return userList;
        }

        private static string GetMasterPassword() {
            const string path = "/cfg/aosrepo/config/master.cfg";
            return !File.Exists(path) ? string.Empty : File.ReadAllText(path.Trim());
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context) {
            var users = Users().ToList();
            var userRecord = users.FirstOrDefault(u => u.Id == identifier);
            return userRecord == null
                       ? null
                       : new UserIdentity { UserName = userRecord.MasterUsername };
        }

        public static string GetUserEmail(Guid identifier) {
            var user = Users().FirstOrDefault(u => u.Id == identifier);
            return user?.MasterUsername;
        }

        public static Guid? ValidateUser(string userIdentity, string password) {
            var validUser = Users().FirstOrDefault(_ => _.MasterUsername == userIdentity);
            var validClaim = validUser?.Claims.FirstOrDefault(_ => _.Key == "master-password" && _.Value == password);
            if (validClaim == null) {
                return null;
            }
            return Guid.Parse(validUser.MasterGuid);
        }
    }
}