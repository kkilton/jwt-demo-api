using System.Web;
using System.Web.Http.Filters;

namespace JWTDemo.Authentication
{
    public class JWTTokenActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(HttpActionExecutedContext actionContext)
        {
            var identity = HttpContext.Current.User as JWTAuthenticationIdentity;

            if (!string.IsNullOrWhiteSpace(identity?.Name))
            {
                //response could be null during an exception
                //create a new token which is the same as extending the token timeout and pass it back
                actionContext.Response?.Headers.Add("Token", AuthenticationModule.GenerateTokenForUser(identity));
                HttpContext.Current.Response.Cookies.Clear();
            }

            base.OnActionExecuted(actionContext);
        }
    }
}
