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
            var userList = new List<UserEntity.UserEntityModel> {
                new UserEntity.UserEntityModel {
                    _Id = "00000000-0000-0000-0000-000000000500",
                    MasterGuid = "00000000-0000-0000-0000-000000000500",
                    MasterUsername = "master",
                    IsEnabled = true,
                    Claims = new List<UserEntity.UserEntityModel.Claim> {
                        new UserEntity.UserEntityModel.Claim {
                            ClaimGuid = "00000000-0000-0000-0000-000000000500",
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
            var path = "/cfg/aosrepo/config/master.cfg";
            if (!File.Exists(path)) {
                return string.Empty;
            }
            return File.ReadAllText(path.Trim());
        }

        public IUserIdentity GetUserFromIdentifier(Guid identifier, NancyContext context) {
            var userRecord = Users().FirstOrDefault(u => u.Guid == identifier);
            return userRecord == null
                       ? null
                       : new UserIdentity { UserName = userRecord.MasterUsername };
        }

        public static string GetUserEmail(Guid identifier) {
            var user = Users().FirstOrDefault(u => u.Guid == identifier);
            return user?.MasterUsername;
        }

        public static Guid? ValidateUser(string userIdentity, string password) {
            var validUser = Users().FirstOrDefault(_ => _.MasterUsername == userIdentity);
            if (validUser == null) {
                return null;
            }
            var validClaim = validUser.Claims.FirstOrDefault(_ => _.Key == "master-password" && _.Value == password);
            if (validClaim == null) {
                return null;
            }
            return Guid.Parse(validUser.MasterGuid);
        }
    }
}