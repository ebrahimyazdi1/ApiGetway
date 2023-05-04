using Gss.ApiGateway.Services.Ocelot;
using Microsoft.Extensions.Options;
using MMLib.SwaggerForOcelot.Configuration;
using RouteOptions = MMLib.SwaggerForOcelot.Configuration.RouteOptions;

namespace Gss.ApiGateway.Infrastructure.SwaggerForOcelot
{
    public class SwaggerRouteOptionsProvider : IOptions<List<RouteOptions>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SwaggerRouteOptionsProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public List<RouteOptions> Value
        {
            get
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _swaggerForOcelotService = scope.ServiceProvider.GetRequiredService<ISwaggerForOcelotService>();
                    return _swaggerForOcelotService.GetSwaggerRouteOptions();
                }
            }
        }
    }
}
