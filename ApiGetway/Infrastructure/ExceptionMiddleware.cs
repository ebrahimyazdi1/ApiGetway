using Gss.ApiGateway.Models.Errors;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Infrastructure
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ExceptionMiddleware> _logger;
        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment env, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
                var error = httpContext.Items.Errors().FirstOrDefault();
                if (error == null)
                {
                    return;
                }

                var message = error.Message;
                if (error.HttpStatusCode >= 500)
                {
                    message = _env.IsDevelopment() ? error.ToString() : "Something went wrong";
                }

                await HandleExceptionAsync(httpContext, message, error.HttpStatusCode);
            }
            catch (OcelotSwaggerUnauthorizedException ex)
            {
                if (ex.ShouldAskUserPassword)
                {
                    httpContext.Response.Headers.Add("WWW-Authenticate", "Basic");
                }

                await HandleExceptionAsync(httpContext, "You are not authorized to see this page", 401);
            }
            catch (OcelotSwaggerEmptyRouteException)
            {
                //ignore
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occured in api gateway");
                await HandleExceptionAsync(httpContext, _env.IsDevelopment() ? ex.ToString() : "Something went wrong with gateway", 500);
            }
        }
        private Task HandleExceptionAsync(HttpContext context, string message, int statusCode)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;
            return context.Response.WriteAsJsonAsync(new
            {
                StatusCode = context.Response.StatusCode,
                Message = message
            });
        }
    }
    public static class ExceptionMiddlewareExtensions
    {
        public static void ConfigureCustomExceptionMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
