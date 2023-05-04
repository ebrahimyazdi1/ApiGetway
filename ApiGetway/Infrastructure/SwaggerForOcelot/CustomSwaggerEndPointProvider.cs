using Gss.ApiGateway.Models.Errors;
using Gss.ApiGateway.Services.Ocelot;
using Kros.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MMLib.SwaggerForOcelot.Configuration;
using MMLib.SwaggerForOcelot.Repositories;
using System.Collections.Generic;
using System.Linq;

namespace Gss.ApiGateway.Infrastructure.SwaggerForOcelot
{
    public class CustomSwaggerEndPointProvider : ISwaggerEndPointProvider
    {
        private Dictionary<string, SwaggerEndPointOptions> _swaggerEndPoints => Init();
        private readonly IOptions<List<SwaggerEndPointOptions>> _swaggerEndPointsOptions;
        private readonly OcelotSwaggerGenOptions _options;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="SwaggerEndPointProvider"/> class.
        /// </summary>
        /// <param name="swaggerEndPoints">The swagger end points.</param>
        public CustomSwaggerEndPointProvider(
            IOptions<List<SwaggerEndPointOptions>> swaggerEndPoints,
            OcelotSwaggerGenOptions options, IServiceScopeFactory serviceScopeFactory)
        {
            _swaggerEndPointsOptions = Check.NotNull(swaggerEndPoints, nameof(swaggerEndPoints));
            _options = options;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <inheritdoc/>
        public IReadOnlyList<SwaggerEndPointOptions> GetAll()
        {
            return _swaggerEndPoints.Values.ToList();
        }

        /// <inheritdoc/>
        public SwaggerEndPointOptions GetByKey(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new OcelotSwaggerEmptyRouteException();
            }

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var _swaggerForOcelotService = scope.ServiceProvider.GetRequiredService<ISwaggerForOcelotService>();
                if (!string.IsNullOrEmpty(key) && !_swaggerForOcelotService.UserHasAccessToThisRoute(key))
                {
                    throw new OcelotSwaggerUnauthorizedException(false);
                }

                return _swaggerEndPoints[$"/{key}"];
            }
        }

        private Dictionary<string, SwaggerEndPointOptions> Init()
        {
            var ret = _swaggerEndPointsOptions.Value.ToDictionary(p => $"/{p.KeyToPath}", p => p);

            if (_options.GenerateDocsForAggregates)
            {
                AddEndpoint(ret, CustomOcelotSwaggerGenOptions.AggregatesKey, "Aggregates");
            }

            if (_options.GenerateDocsForGatewayItSelf)
            {
                AddEndpoint(ret, CustomOcelotSwaggerGenOptions.GatewayKey, "Gateway");
            }

            return ret;
        }

        private static void AddEndpoint(Dictionary<string, SwaggerEndPointOptions> ret, string key, string description)
        {
            ret.Add($"/{key}", new SwaggerEndPointOptions()
            {
                Key = key,
                TransformByOcelotConfig = false,
                Config = new List<SwaggerEndPointConfig>() {
                    new SwaggerEndPointConfig()
                    {
                        Name = description,
                        Version = key,
                        Url = ""
                    }
                }
            });
        }
    }
}
