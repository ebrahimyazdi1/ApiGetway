using Gss.ApiGateway.Models.Ocelot;
using Ocelot.Configuration.File;
using System;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Services.Ocelot
{
    public interface IOcelotConfigService
    {
        Task<OcelotConfigEntity> GetLatestVersionAsync();

        Task<OcelotConfigEntity> GetSpecificVersionAsync(Version version);

        Task<OcelotConfigEntity> SetAsync(FileConfiguration config);

        Task<OcelotConfigEntity> SetAsync(FileConfiguration config, Version version);
    }
}
