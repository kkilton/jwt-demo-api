using System.Web.Http;
using JWTDemo.Authentication;

namespace JWTDemo.Controllers
{
    [JWTAuthenticationFilter]
    [JWTTokenActionFilter]
    public class TestController : ApiController
    {
        // GET api/<controller>
        public string Get()
        {
            return "Don't share this semi-secure data!";
        }

        [Authorize(Roles = "admin")]
        [Route("api/test/GetAdminData")]
        public string GetAdminData()
        {
            return "Don't share this really secure admin data!";
        }

        [AllowAnonymous]
        [Route("api/test/GetUnsecureData")]
        public string GetUnsecureData()
        {
            return "Anyone can see this data!";
        }
    }
}