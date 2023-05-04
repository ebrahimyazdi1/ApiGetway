using ApiGetway.Infrastructure.IdentityServer;
using Gss.ApiGateway.Auth;
using Gss.ApiGateway.Data;
using Gss.ApiGateway.Infrastructure;
using Gss.ApiGateway.Infrastructure.Ocelot;
using Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider;
using Gss.ApiGateway.Infrastructure.SwaggerForOcelot;
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
            //builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            builder.Services.AddOcelot(builder.Configuration);
            builder.Services.AddMvcCore().AddApiExplorer();
            builder.Services.AddDbContext<GssDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApiGetway"));
            });
            //options.ConfigureDbContext = b => b.UseOracle(connectionString,
            //            sql => sql.MigrationsAssembly(migrationsAssembly));
            //ConfigureServiceHelper configureService = new ConfigureServiceHelper(configureService);
            //AddDataSeeders(builder.Services);
            //AddIdentity(builder.Services);

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
            try
            {

              
 
                 app.UseOcelot(OcelotPipelineConfigurationBuilder.GetOcelotPipelineConfiguration());
            }
            catch (Exception ex)
            {

                throw;
            }
            app.UseIdentityServer();
            app.Map("/identity", idsrvApp =>
            {
                idsrvApp.UseIdentityServer();
            });

            app.Run();
        }
    }
}