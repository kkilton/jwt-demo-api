using System.Web.Http;
using JWTDemo.Authentication;
using JWTDemo.Models;

namespace JWTDemo.Controllers
{
    [AllowAnonymous]
    [JWTTokenActionFilter]
    public class AuthenticationController : ApiController
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public string Get()
        {
            return "Hi!";
        }

        public void Post(LoginModel login)
        {
            _authenticationService.AuthenticateUser(login.Username, login.Password);
        }
    }
}
