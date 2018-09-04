using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Web;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;

namespace JWTDemo.Authentication
{
    /// <summary>
    /// Needs the following 
    /// </summary>
    public class AuthenticationModule
    {
        private static readonly double Timeout;
        private static readonly string Audience;
        private static readonly string Issuer;
        private static readonly SecurityKey SigningKey;

        /// <summary>
        /// Ctor
        /// </summary>
        static AuthenticationModule()
        {
            var tokenSettings = TokenSettings.GetTokenSettings()[0];
            if (string.IsNullOrWhiteSpace(tokenSettings.CommunicationKey))
            {
                throw new ConfigurationErrorsException("Missing 'TokenCommunicationKey' from the AppSettings");
            }
            if (string.IsNullOrWhiteSpace(tokenSettings.Issuer))
            {
                throw new ConfigurationErrorsException("Missing 'Issuer' from the AppSettings");
            }
            if (string.IsNullOrWhiteSpace(tokenSettings.Audience))
            {
                throw new ConfigurationErrorsException("Missing 'Audience' from the AppSettings");
            }
            if (string.IsNullOrWhiteSpace(tokenSettings.Timeout))
            {
                throw new ConfigurationErrorsException("Missing 'Timeout' from the AppSettings");
            }

            double.TryParse(tokenSettings.Timeout, out Timeout);
            Issuer = tokenSettings.Issuer;
            Audience = tokenSettings.Audience;
            var communicationKey = tokenSettings.CommunicationKey;
            SigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(communicationKey));
        }

        /// <summary>
        /// Generate a token for a user based on a known user
        /// </summary>
        /// <param name="identity">the user's identity and roles</param>
        /// <returns>a JWT token</returns>
        public static string GenerateTokenForUser(JWTAuthenticationIdentity identity)
        {
            var now = DateTime.UtcNow;
            var signingCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256Signature);

            var claimsIdentity = new ClaimsIdentity(new List<Claim>
            {
                new Claim("firstName", identity.FirstName),
                new Claim("lastName", identity.LastName),
                new Claim("userName", identity.UserName)
            }, identity.AuthenticationType);

            foreach (var role in identity.Roles)
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = Audience,
                Issuer = Issuer,
                Subject = claimsIdentity,
                SigningCredentials = signingCredentials,
                Expires = now.AddMinutes(Timeout),
                IssuedAt = now
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var plainToken = tokenHandler.CreateToken(securityTokenDescriptor);
            var signedAndEncodedToken = tokenHandler.WriteToken(plainToken);

            return signedAndEncodedToken;

        }

        /// Using the same key used for signing token, user payload is generated back
        public JwtSecurityToken GenerateUserClaimFromJWT(string authToken)
        {

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidAudiences = new[]
                      {
                          Audience
                      },

                ValidIssuers = new[]
                  {
                      Issuer
                  },
                IssuerSigningKey = SigningKey
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            IdentityModelEventSource.ShowPII = true;
            SecurityToken validatedToken;

            try
            {

                tokenHandler.ValidateToken(authToken, tokenValidationParameters, out validatedToken);
            }
            catch (Exception ex)
            {
                return null;
            }

            return validatedToken as JwtSecurityToken;

        }

        public JWTAuthenticationIdentity PopulateUserIdentity(JwtSecurityToken userPayloadToken)
        {
            string firstName = userPayloadToken.Claims.FirstOrDefault(c => c.Type == "firstName")?.Value;
            string lastName = userPayloadToken.Claims.FirstOrDefault(c => c.Type == "lastName")?.Value;
            string userName = userPayloadToken.Claims.FirstOrDefault(c => c.Type == "userName")?.Value;

            var roles = userPayloadToken.Claims.Where(c => c.Type == "role").Select(c => c.Value).ToList();
            return new JWTAuthenticationIdentity(userName)
            {
                UserName = userName,
                FirstName = firstName,
                LastName = lastName,
                Roles = roles
            };
        }

        public string FetchFromHeader(HttpRequestMessage request)
        {
            string requestToken = null;

            var authRequest = request.Headers.Authorization;
            if (authRequest != null)
            {
                requestToken = authRequest.Parameter;
            }

            return requestToken;
        }

        public static void SetUserIdentity(JWTAuthenticationIdentity identity)
        {
           var genericPrincipal = new GenericPrincipal(identity, identity.Roles.ToArray());
            Thread.CurrentPrincipal = genericPrincipal;
            var authenticationIdentity = Thread.CurrentPrincipal.Identity as JWTAuthenticationIdentity;
            if (!string.IsNullOrEmpty(authenticationIdentity?.UserName))
            {
                authenticationIdentity.UserName = identity.UserName;
                authenticationIdentity.Roles = identity.Roles;
            }

            HttpContext.Current.User = authenticationIdentity;
        }
    }
}
