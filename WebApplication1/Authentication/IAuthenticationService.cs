
namespace JWTDemo.Authentication
{
    public interface IAuthenticationService
    {
        JWTAuthenticationIdentity AuthenticateUser(string userName, string password);
    }
}
