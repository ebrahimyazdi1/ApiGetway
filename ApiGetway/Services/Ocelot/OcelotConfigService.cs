using Gss.ApiGateway.Data;
using Gss.ApiGateway.Models.Errors;
using Gss.ApiGateway.Models.Ocelot;
using Ocelot.Configuration.File;
using System.Data.Entity;

namespace Gss.ApiGateway.Services.Ocelot
{
    public class OcelotConfigService : IOcelotConfigService
    {
        private readonly GssDbContext _dbContext;

        public OcelotConfigService(GssDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<OcelotConfigEntity> GetLatestVersionAsync()
        {
            var config = await _dbContext.OcelotFileConfigurations.AsNoTracking()
                .OrderByDescending(x => x.Version).FirstOrDefaultAsync(x => x.IsActive);
            if (config == null)
            {
                throw new OcelotConfigNotSetException();
            }

            return config;
        }

        public async Task<OcelotConfigEntity> GetSpecificVersionAsync(Version version)
        {
            var config = await _dbContext.OcelotFileConfigurations.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Version == version && x.IsActive);
            if (config == null)
            {
                throw new OcelotConfigNotSetException();
            }

            return config;
        }

        public async Task<OcelotConfigEntity> SetAsync(FileConfiguration config)
        {
            var obj = new OcelotConfigEntity { CreatedOn = DateTime.UtcNow, Payload = config, IsActive = true };
            try
            {
                var latestVersion = await GetLatestVersionAsync();
                obj.Version = new Version(latestVersion.Version.Major, latestVersion.Version.Minor + 1);
            }
            catch (OcelotConfigNotSetException)
            {
                obj.Version = new Version(1, 0);
            }
            var entity = _dbContext.OcelotFileConfigurations.Add(obj);
            await _dbContext.SaveChangesAsync();
            return entity.Entity;
        }

        public async Task<OcelotConfigEntity> SetAsync(FileConfiguration config, Version version)
        {
            var obj = new OcelotConfigEntity { CreatedOn = DateTime.UtcNow, Version = version, Payload = config, IsActive = true };
            var entity = _dbContext.OcelotFileConfigurations.Add(obj);
            await _dbContext.SaveChangesAsync();
            return entity.Entity;
        }
    }
}
