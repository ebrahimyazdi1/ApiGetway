using Gss.ApiGateway.Services.Ocelot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MMLib.SwaggerForOcelot.Configuration;
using MMLib.SwaggerForOcelot.Repositories;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gss.ApiGateway.Infrastructure.SwaggerForOcelot
{
    public static class SwaggerForOcelotExtensions
    {
        public static IServiceCollection AddSwaggerForGssOcelot(this IServiceCollection services, IConfiguration configuration, Action<OcelotSwaggerGenOptions> ocelotSwaggerSetup = null, Action<SwaggerGenOptions> swaggerSetup = null)
        {
            services.AddScoped<ISwaggerForOcelotService, SwaggerForOcelotService>();
            var serviceCollection = services.AddSwaggerForOcelot(configuration, ocelotSwaggerSetup, swaggerSetup);
            serviceCollection.AddSingleton<ISwaggerEndPointProvider, CustomSwaggerEndPointProvider>();
            serviceCollection.AddSingleton<IOptions<List<MMLib.SwaggerForOcelot.Configuration.RouteOptions>>, SwaggerRouteOptionsProvider>();
            serviceCollection.AddSingleton<IOptions<List<SwaggerEndPointOptions>>, SwaggerEndPointOptionsProvider>();

            services.Remove(services.FirstOrDefault(x => x.ServiceType == typeof(ISwaggerEndPointProvider)));

            return serviceCollection;
        }
    }
}
