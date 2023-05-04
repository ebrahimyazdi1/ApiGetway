using Gss.ApiGateway.Services.Ocelot;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MMLib.SwaggerForOcelot.Configuration;
using System.Collections.Generic;

namespace Gss.ApiGateway.Infrastructure.SwaggerForOcelot
{
    public class SwaggerEndPointOptionsProvider : IOptions<List<SwaggerEndPointOptions>>
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public SwaggerEndPointOptionsProvider(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public List<SwaggerEndPointOptions> Value
        {
            get
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var _swaggerForOcelotService = scope.ServiceProvider.GetRequiredService<ISwaggerForOcelotService>();
                    var endpoints = _swaggerForOcelotService.GetSwaggerEndPointOptions();
                    return endpoints;
                }
            }
        }
    }
}
