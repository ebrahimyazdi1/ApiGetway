using ApiGetway.Infrastructure.IdentityServer;
using Gss.ApiGateway.Auth;
using Gss.ApiGateway.Data;
using Gss.ApiGateway.Infrastructure;
using Gss.ApiGateway.Infrastructure.Ocelot;
using Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider;
using Gss.ApiGateway.Infrastructure.SwaggerForOcelot;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;


namespace ApiGetway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
          //  builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //return Host.CreateDefaultBuilder(args)
            //                .ConfigureWebHostDefaults(webBuilder =>
            //                {
            //                    webBuilder.UseStartup<Startup>();
            //                })
            //                .ConfigureAppConfiguration(configuration =>
            //                {
            //                    configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //                    configuration.AddJsonFile(
            //                        $"appsettings.{environment}.json",
            //                        optional: true);
            //                    EnsureDatabaseAndMigrations(configuration.Build());
            //                })
            //                .UseSerilog();




            builder.Services.AddOcelot(builder.Configuration);
            builder.Services.AddMvcCore().AddApiExplorer();
            builder.Services.AddDbContext<GssDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApiGetway"));
            });
           
            builder.Services.AddOcelot(builder.Configuration).AddConfigStoredInSQLServer().AddPolly()
                .AddDelegatingHandler<GssGlobalHttpDelegatingHandler>(true);
            builder.Services.AddSwaggerForGssOcelot(builder.Configuration);
            builder.Services.AddIdentityServer()
              .AddDeveloperSigningCredential()
              //.AddInMemoryApiResources(Config.IdentityResources)
              .AddInMemoryClients(Config.Clients)
              .AddInMemoryApiScopes(Config.ApiScopes);
            //   builder.Services.AddAuthentication()
            //.AddJwtBearer("OAuthJwtBearer", builder.Ser);
            var app = builder.Build();
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.ConfigureCustomExceptionMiddleware();
            app.UseOcelotSwaggerAuthenticationMiddleware();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
                // opt.InjectStylesheet("/swagger-ui/swagger-ui.css");
            });
            app.UseStaticFiles();
            app.UseOcelot(OcelotPipelineConfigurationBuilder.GetOcelotPipelineConfiguration());
            app.UseIdentityServer();
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer();
            });

            app.Run();
        }
    }
}