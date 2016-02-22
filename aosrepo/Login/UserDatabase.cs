using Nancy;
using Nancy.Authentication.Forms;
using Nancy.Security;
using System;
using System.Collections.Generic;
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
                    MasterGuid = "master",
                    MasterUsername = "master",
                    IsEnabled = true,
                    Claims = new List<UserEntity.UserEntityModel.Claim> {
                        new UserEntity.UserEntityModel.Claim {
                            ClaimGuid = "00000000-0000-0000-0000-000000000500",
                            Type= UserEntity.ClaimType.UserPassword,
                            Key = "master-password",
                            Value= "fake_password123"
                        }
                    }
                }
            };
            return userList;
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
            


            if (userIdentity == "fakeadmin" && password == "fake_password123") {
                return Guid.Parse("00000000-0000-0000-0000-000000000500");
            }
            else return null;
        }
    }
}