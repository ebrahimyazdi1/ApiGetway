using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Ocelot.Configuration.Creator;
using Ocelot.Configuration.File;
using Ocelot.Configuration.Repository;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gss.ApiGateway.Infrastructure.Ocelot.SqlServerProvider
{
    public class CustomFileConfigurationPoller : IHostedService, IDisposable
    {
        private readonly IFileConfigurationRepository _repo;
        private string _previousAsJson;
        private Timer _timer;
        private bool _polling;
        private readonly IFileConfigurationPollerOptions _options;
        private readonly IInternalConfigurationRepository _internalConfigRepo;
        private readonly IInternalConfigurationCreator _internalConfigCreator;

        public CustomFileConfigurationPoller(
            IFileConfigurationRepository repo,
            IFileConfigurationPollerOptions options,
            IInternalConfigurationRepository internalConfigRepo,
            IInternalConfigurationCreator internalConfigCreator)
        {
            _internalConfigRepo = internalConfigRepo;
            _internalConfigCreator = internalConfigCreator;
            _options = options;
            _repo = repo;
            _previousAsJson = "";
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(async x =>
            {
                if (_polling)
                {
                    return;
                }

                _polling = true;
                await Poll();
                _polling = false;
            }, null, _options.Delay, _options.Delay);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        private async Task Poll()
        {
            var fileConfig = await _repo.Get();

            if (fileConfig.IsError)
            {
                return;
            }

            var asJson = ToJson(fileConfig.Data);

            if (!fileConfig.IsError && asJson != _previousAsJson)
            {
                var config = await _internalConfigCreator.Create(fileConfig.Data);

                if (!config.IsError)
                {
                    _internalConfigRepo.AddOrReplace(config.Data);
                }

                _previousAsJson = asJson;
            }
        }

        /// <summary>
        /// We could do object comparison here but performance isnt really a problem. This might be an issue one day!
        /// </summary>
        /// <returns>hash of the config</returns>
        private string ToJson(FileConfiguration config)
        {
            var currentHash = JsonConvert.SerializeObject(config);
            return currentHash;
        }

        public void Dispose()
        {
            _timer?.Dispose();
            _timer = null;
        }
    }
}
