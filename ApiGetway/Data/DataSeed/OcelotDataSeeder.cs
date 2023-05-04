using Gss.ApiGateway.Models.Errors;
using Gss.ApiGateway.Services.Ocelot;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Data.DataSeed
{
    public class OcelotDataSeeder : IDataSeeder
    {
        private readonly IOcelotConfigService _ocelotConfigService;

        public OcelotDataSeeder(IOcelotConfigService ocelotConfigService)
        {
            _ocelotConfigService = ocelotConfigService;
        }

        public async Task SeedData()
        {
            try
            {
                var latestVersion = await _ocelotConfigService.GetLatestVersionAsync();
            }
            catch (OcelotConfigNotSetException)
            {
                await _ocelotConfigService.SetAsync(new Ocelot.Configuration.File.FileConfiguration());
            }

        }
    }
}
