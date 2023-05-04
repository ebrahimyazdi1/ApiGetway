namespace Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider
{
    using global::Ocelot.Cache;
    using global::Ocelot.Configuration.File;
    using global::Ocelot.Configuration.Repository;
    using global::Ocelot.Logging;
    using global::Ocelot.Responses;
    using Gss.ApiGateway.Services.Ocelot;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    public class SqlServerFileConfigurationRepository : IFileConfigurationRepository
    {
        private const string _configurationKey = "InternalConfiguration";
        private readonly IOcelotCache<FileConfiguration> _cache;
        private readonly IOcelotLogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;
        public SqlServerFileConfigurationRepository(
            IOcelotCache<FileConfiguration> cache,
            IInternalConfigurationRepository repo,
            IOcelotLoggerFactory loggerFactory, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _logger = loggerFactory.CreateLogger<SqlServerFileConfigurationRepository>();
            _cache = cache;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
        }

        public async Task<Response<FileConfiguration>> Get()
        {
            try
            {
                var config = _cache.Get(_configurationKey, _configurationKey);
                if (config == null)
                {
                    using (var scopeFactory = _serviceScopeFactory.CreateScope())
                    {
                        var service = scopeFactory.ServiceProvider.GetRequiredService<IOcelotConfigService>();
                        var configEntity = await service.GetLatestVersionAsync();
                        config = configEntity.Payload;

                        var cacheTimeout = _configuration["OcelotCacheTimeout"];
                        _cache.AddAndDelete(_configurationKey, config, TimeSpan.Parse(cacheTimeout), null);
                    };
                }
                return new OkResponse<FileConfiguration>(config);
            }
            catch (Exception ex)
            {
                _logger.LogError("Getting ocelot config raise an error", ex);
                return new OkResponse<FileConfiguration>(new FileConfiguration());
            }

        }

        public async Task<Response> Set(FileConfiguration ocelotConfiguration)
        {
            try
            {
                _logger.LogInformation("Setting route rules");
                var cfgPayload = JsonConvert.SerializeObject(ocelotConfiguration, Formatting.Indented);
                using (var scopeFactory = _serviceScopeFactory.CreateScope())
                {
                    var service = scopeFactory.ServiceProvider.GetRequiredService<IOcelotConfigService>();

                    await service.SetAsync(ocelotConfiguration);

                    var cacheTimeout = _configuration["OcelotCacheTimeout"];
                    _cache.AddAndDelete(_configurationKey, ocelotConfiguration, TimeSpan.Parse(cacheTimeout), null);
                }
                return new OkResponse();
            }
            catch (Exception ex)
            {
                _logger.LogError("Setting ocelot config raise an error", ex);
                return new ErrorResponse(new SetConfigInSqlServerError($"Failed to set FileConfiguration in sql server, error message:{ex.Message}", 500));
            }
        }
    }
}
