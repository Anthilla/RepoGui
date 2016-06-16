using System;
using System.Collections.Generic;

namespace aosrepo.Login {
    public class UserEntity {
        public static ClaimType ConvertClaimType(string claimString) {
            switch (claimString) {
                case "identity":
                    return ClaimType.UserIdentity;
                case "password":
                    return ClaimType.UserPassword;
                case "token":
                    return ClaimType.UserToken;
                case "pin":
                    return ClaimType.UserPin;
                default:
                    return ClaimType.Other;
            }
        }

        public enum ClaimType : byte {
            UserIdentity = 1,
            UserPassword = 2,
            UserToken = 3,
            UserPin = 4,
            Other = 99
        }

        public enum ClaimMode : byte {
            Antd = 1,
            ActiveDirectory = 3,
            AnthillaSp = 4,
            Null = 98,
            Other = 99
        }

        public class UserEntityModel {
            public Guid Id { get; set; } = Guid.NewGuid();
            public string MasterGuid { get; set; }
            public string MasterUsername { get; set; }
            public string MasterAlias { get; set; }
            public bool IsEnabled { get; set; }
            public IEnumerable<Claim> Claims { get; set; }

            public string PublicKey { get; set; }
            public string PrivateKey { get; set; }

            public class Claim {
                public string ClaimGuid { get; set; }
                public string ClaimUserGuid { get; set; }
                public ClaimType Type { get; set; }
                public ClaimMode Mode { get; set; }
                public string Key { get; set; }
                public string Value { get; set; }
            }
        }

        public enum AuthenticationStatus : byte {
            Ok = 0,
            UserDoesNotExists = 1,
            UserNotEnabled = 2,
            WrongCredential = 3,
            WrongPassword = 4,
            Error = 99
        }
    }
}