using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;

namespace JWTDemo.Authentication
{
    /// <summary>
    /// The user's identity and roles
    /// </summary>
    public class JWTAuthenticationIdentity : GenericIdentity, IPrincipal
    {
        public JWTAuthenticationIdentity(string userName) : base(userName)
        {
            UserName = userName;
        }

        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public bool IsInRole(string role)
        {
            return Roles.Contains(role);
        }

        public IIdentity Identity
        {
            get { return this; }
        }
    }
}