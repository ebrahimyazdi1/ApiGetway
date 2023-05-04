
namespace Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider
{
    using global::Ocelot.Cache;
    using global::Ocelot.Configuration.File;
    using global::Ocelot.Configuration.Repository;
    using global::Ocelot.DependencyInjection;
    using Gss.ApiGateway.Services.Ocelot;
    using Microsoft.Extensions.DependencyInjection;
    using System.Linq;

    public static class OcelotBuilderExtensions
    {
        public static IOcelotBuilder AddConfigStoredInSQLServer(this IOcelotBuilder builder)
        {
            builder.Services.AddScoped<IOcelotConfigService, OcelotConfigService>();
            builder.Services.AddSingleton(SqlServerMiddlewareConfigurationProvider.Get);
            builder.Services.AddSingleton<IFileConfigurationRepository, SqlServerFileConfigurationRepository>();
            builder.Services.AddSingleton<IOcelotCache<FileConfiguration>, OcelotConfigCache>();
            builder.Services.AddHostedService<CustomFileConfigurationPoller>();

            builder.Services.Remove(builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IFileConfigurationRepository)));
            builder.Services.Remove(builder.Services.FirstOrDefault(x => x.ServiceType == typeof(IOcelotCache<FileConfiguration>)));
            return builder;
        }
    }
}
