using Ocelot.Middleware;

namespace Gss.ApiGateway.Infrastructure.Ocelot
{
    public class OcelotPipelineConfigurationBuilder
    {
        public static OcelotPipelineConfiguration GetOcelotPipelineConfiguration()
        {
            var configuration = new OcelotPipelineConfiguration
            {
                PreErrorResponderMiddleware = async (ctx, next) =>
                {
                    //do whatever!
                    await next.Invoke();
                }
            };
            return configuration;
        }
    }
}
