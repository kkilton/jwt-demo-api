using System.Web.Http;
using System.Web.Http.Cors;
using JWTDemo.Configuration;

namespace JWTDemo
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var corsSettings = CorsSettings.GetCorsSettings()[0];
            var cors = new EnableCorsAttribute(
                origins: corsSettings.Origins,
                headers: corsSettings.Headers,
                methods: corsSettings.Methods,
                exposedHeaders: corsSettings.ExposedHeaders);
            config.EnableCors(cors);
        }
    }
}
