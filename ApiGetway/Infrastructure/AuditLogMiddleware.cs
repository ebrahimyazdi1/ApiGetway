using Audit.Core;
using Gss.ApiGateway.Models.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Ocelot.Infrastructure.RequestData;
using Ocelot.Middleware;
using Ocelot.Request.Middleware;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Infrastructure
{
    public class AuditLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly IRequestScopedDataRepository _requestScopedDataRepository;
        private readonly string[] _ignored_path = new[] { "/swagger", "/favicon" };
        public AuditLogMiddleware(RequestDelegate next, IWebHostEnvironment env, IRequestScopedDataRepository requestScopedDataRepository)
        {
            _next = next;
            _env = env;
            _requestScopedDataRepository = requestScopedDataRepository;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            if (_ignored_path.Any(x => httpContext.Request.Path.Value.StartsWith(x)))
            {
                await _next(httpContext);
                return;
            }
            using (var scope = AuditScope.Create("ocelot-finalized-request", () => null))
            {
                await _next(httpContext);
                AddUsernameToLogfield(scope);
                await AddDownstreamRequestToLogfield(scope);
                await AddDownstreamResponseToLogfield(scope);
                AddErrorsLogfield(scope, httpContext);
                AddRemoteIpToLogfield(scope, httpContext);
            }
        }

        private async Task AddDownstreamRequestToLogfield(AuditScope scope)
        {
            var item = _requestScopedDataRepository.Get<DownstreamRequest>("DownstreamRequest");
            if (!item.IsError)
            {
                var body = await item.Data.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(body))
                {
                    scope.SetCustomField("requestBody", body);
                }

                scope.SetCustomField("request", item.Data);
            }
        }

        private async Task AddDownstreamResponseToLogfield(AuditScope scope)
        {
            var item = _requestScopedDataRepository.Get<DownstreamResponse>("DownstreamResponse");
            if (!item.IsError)
            {
                try
                {
                    if (item.Data is null) return;
                    var byteArray = await item.Data.Content.ReadAsByteArrayAsync();
                    var responseString = Encoding.UTF8.GetString(byteArray, 0, byteArray.Length);
                    if (!string.IsNullOrEmpty(responseString))
                    {
                        scope.SetCustomField("responseString", responseString);
                    }
                }
                catch (System.Exception)
                {
                }
                scope.SetCustomField("responseCode", item.Data?.StatusCode);
            }
        }

        private void AddErrorsLogfield(AuditScope scope, HttpContext httpContext)
        {
            var errors = httpContext.Items.Errors();
            if (errors.Any())
            {
                var error = errors.FirstOrDefault();
                scope.SetCustomField("errors", errors);
                scope.SetCustomField("responseCode", error.HttpStatusCode);
            }
        }

        private void AddUsernameToLogfield(AuditScope scope)
        {
            var item = _requestScopedDataRepository.Get<GssUser>("CurrentUser");
            if (!item.IsError)
            {
                scope.SetCustomField("currentUser", item.Data?.UserName);
            }
        }

        private void AddRemoteIpToLogfield(AuditScope scope, HttpContext httpContext)
        {
            var ip = httpContext.Connection.RemoteIpAddress.ToString();
            scope.SetCustomField("remoteIp", ip);
        }
    }
    public static class AuditLogMiddlewareExtensions
    {
        public static void UseAuditLogMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<AuditLogMiddleware>();
        }
    }
}
