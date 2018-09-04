using System.Collections.Generic;
using System.Net;
using System.Security.Authentication;
using JWTDemo.Authentication;

namespace JWTDemo
{
    public class AuthenticationService : IAuthenticationService
    {
        public AuthenticationService()
        {
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        public JWTAuthenticationIdentity AuthenticateUser(string userName, string password)
        {
            //TODO - actually validate the user
            if (userName == "kkilton")
            {
                var identity = new JWTAuthenticationIdentity(userName)
                {
                    FirstName = "Kris",
                    LastName = "Kilton",
                    UserName = userName,
                    Roles = new List<string> {"admin"}
                };
                AuthenticationModule.SetUserIdentity(identity);
                return identity;
            }

            if (userName == "user")
            {
                var identity = new JWTAuthenticationIdentity(userName)
                {
                    FirstName = "Just",
                    LastName = "User",
                    UserName = userName,
                    Roles = new List<string> { "user" }
                };
                AuthenticationModule.SetUserIdentity(identity);
                return identity;
            }
            throw new AuthenticationException();
        }
    }
}