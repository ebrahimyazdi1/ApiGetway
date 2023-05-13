using Gss.ApiGateway.Infrastructure.Ocelot;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;


namespace ApiGetway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddJsonFile("appsettings.json")
                .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true).AddEnvironmentVariables();
            builder.Services.AddMvcCore().AddApiExplorer();
            // Add rate limiting services
            //builder.Services.AddMemoryCache();
            //builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("RateLimitOptions"));
            //builder.Services.Configure<IpRateLimitPolicies>(builder.Configuration.GetSection("IpRateLimitPolicies"));
            //builder.Services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            //builder.Services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            //builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            //builder.Services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();
            builder.Services.AddLogging(b =>
            {
                b.AddConsole();
            });
            builder.Services.AddOcelot(builder.Configuration);
            builder.Services.AddSwaggerForOcelot(builder.Configuration);
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("GatewayCorsPolicy", builder => {
                    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
                });
            });
            //builder.Services.AddAuthentication();
            //builder.Services.AddAuthorization();
            var app = builder.Build();
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            //app.UseIpRateLimiting();
            //app.UseClientRateLimiting();
            app.UseStaticFiles();
            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
                // opt.InjectStylesheet("/swagger-ui/swagger-ui.css");
            });
            //var ocelotConfig = app.Services.GetService<IConfiguration>().GetSection("GlobalConfiguration:RateLimitOptions").AsEnumerable() ;
            //  var test =  ocelotConfig?.GetChildren()?.AsEnumerable() ;
            app.UseCors("GatewayCorsPolicy");
            app.UseOcelot().Wait();
            //app.UseOcelot(OcelotPipelineConfigurationBuilder.GetOcelotPipelineConfiguration()).Wait();
            app.Run();
        }
    }
}