using Gss.ApiGateway.Models.Errors;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Auth
{
    public class OcelotSwaggerAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;

        public OcelotSwaggerAuthenticationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var path = httpContext.Request.Path.Value.ToString();
            if (!path.StartsWith("/swagger"))
            {
                await _next(httpContext);
                return;
            }

            var authResult = await httpContext.AuthenticateAsync(GssBasicAuthenticationHandler.SchemeName);
            if (authResult.Succeeded)
            {
                await _next(httpContext);
            }
            else
            {
                throw new OcelotSwaggerUnauthorizedException(true);
            }
        }
    }

    public static class OcelotSwaggerAuthenticationMiddlewareExtensions
    {
        public static IApplicationBuilder UseOcelotSwaggerAuthenticationMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<OcelotSwaggerAuthenticationMiddleware>();
        }
    }
}
