using MMLib.SwaggerForOcelot.Configuration;
using System.Collections.Generic;

namespace Gss.ApiGateway.Services.Ocelot
{
    public interface ISwaggerForOcelotService
    {
        List<MMLib.SwaggerForOcelot.Configuration.RouteOptions> GetSwaggerRouteOptions();

        List<SwaggerEndPointOptions> GetSwaggerEndPointOptions();

        bool UserHasAccessToThisRoute(string swaggerKey);
    }
}
