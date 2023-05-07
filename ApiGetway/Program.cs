using ApiGetway.Infrastructure.IdentityServer;
using Gss.ApiGateway.Auth;
using Gss.ApiGateway.Data;
using Gss.ApiGateway.Extensions;
using Gss.ApiGateway.Infrastructure;
using Gss.ApiGateway.Infrastructure.Ocelot;
using Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider;
using Gss.ApiGateway.Infrastructure.SwaggerForOcelot;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Authorization;
using Ocelot.DependencyInjection;
using Ocelot.Infrastructure.RequestData;
using Ocelot.Middleware;
using Ocelot.Values;


namespace ApiGetway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            builder.Services.AddOcelot(builder.Configuration);
            builder.Services.AddSwaggerForOcelot(builder.Configuration);
            builder.Services.AddMvcCore().AddApiExplorer();
            //Action<JwtBearerOptions> options = o =>
            //{
            //    o.Authority = builder.Configuration["OAuth:Authority"];
            //    o.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateAudience = false,
            //        ValidateLifetime = true,
            //        ValidateIssuer = true,
            //        ValidIssuer = builder.Configuration["OAuth:Authority"],

            //    };
            //    o.Events = new JwtBearerEvents
            //    {
            //        OnAuthenticationFailed = (x) =>
            //        {
            //            var requestScopedDataRepository = x.HttpContext.RequestServices.GetRequiredService<IRequestScopedDataRepository>();
            //            requestScopedDataRepository.AddError(new UnauthorizedError("Unauthorized request to oauth provider"));
            //            return Task.CompletedTask;
            //        }
            //    };
            //};

            //builder.Services.AddAuthentication()
            //    .AddJwtBearer("OAuthJwtBearer", options);
            //builder.Services.AddAuthentication();
            //builder.Services.AddAuthorization();
            var app = builder.Build();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
                // opt.InjectStylesheet("/swagger-ui/swagger-ui.css");
            });
            app.UseOcelot(OcelotPipelineConfigurationBuilder.GetOcelotPipelineConfiguration());
            app.Run();
        }
    }
}