using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace JWTDemo.Authentication
{
    public class JWTAuthenticationFilterAttribute : AuthorizationFilterAttribute
    {
        public override Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            if (!IsUserAutheticated(actionContext))
                ShowAuthenticationError(actionContext);

            return Task.FromResult<object>(null);
        }

        public bool IsUserAutheticated(HttpActionContext actionContext)
        {
            var skipAuthorization =
                actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>(true).Any()
                || actionContext.ActionDescriptor.ControllerDescriptor
                    .GetCustomAttributes<AllowAnonymousAttribute>(true).Any();

            if (skipAuthorization)
                return true;

            var auth = new AuthenticationModule();
            var authHeader = auth.FetchFromHeader(actionContext.Request); //fetch authorization token from header

            if (authHeader != null)
            {
                //validate header token and create a JwtSecurityToken
                var userPayloadToken = auth.GenerateUserClaimFromJWT(authHeader);

                if (userPayloadToken != null)
                {
                    var identity = auth.PopulateUserIdentity(userPayloadToken);
                    AuthenticationModule.SetUserIdentity(identity);
                    return true;
                }
            }
            return false;
        }

        private static void ShowAuthenticationError(HttpActionContext filterContext)
        {
            filterContext.Response =
                filterContext.Request.CreateResponse(HttpStatusCode.Unauthorized,
                    new {Code = 401, Message = "Unable to access, Please login again"});
        }
    }
}