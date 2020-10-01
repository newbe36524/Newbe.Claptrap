using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public class DataBaseService : IDataBaseService
    {
        private readonly ILogger<DataBaseService> _logger;
        private readonly IDockerComposeService _dockerComposeService;

        public DataBaseService(
            ILogger<DataBaseService> logger,
            IDockerComposeService dockerComposeService)
        {
            _logger = logger;
            _dockerComposeService = dockerComposeService;
        }

        private string GetContextDirectory(DatabaseType databaseType)
        {
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory!,
                "Docker",
                "Db",
                databaseType.ToString("G"));
        }

        private string GetContextDataDirectory(DatabaseType databaseType)
        {
            return Path.Combine(GetContextDirectory(databaseType), "Data");
        }

        public async Task StartAsync(DatabaseType databaseType, int preparingSleepInSec)
        {
            switch (databaseType)
            {
                case DatabaseType.PostgreSQL:
                case DatabaseType.MySql:
                case DatabaseType.MongoDB:
                    var context = GetContextDirectory(databaseType);
                    await _dockerComposeService.DownAsync(context);
                    var dataDir = GetContextDataDirectory(databaseType);
                    if (Directory.Exists(dataDir))
                    {
                        _logger.LogInformation("found {dir}, delete it.", dataDir);
                        Directory.Delete(dataDir, true);
                    }

                    await _dockerComposeService.UpAsync(context);
                    // sleep 30s for waiting database setting up
                    _logger.LogInformation("start to sleep in {times} seconds", preparingSleepInSec);
                    await Task.Delay(TimeSpan.FromSeconds(preparingSleepInSec));
                    _logger.LogInformation("sleep done");
                    break;
                case DatabaseType.SQLite:
                case DatabaseType.Known:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public async Task CleanAsync(DatabaseType databaseType)
        {
            switch (databaseType)
            {
                case DatabaseType.PostgreSQL:
                case DatabaseType.MySql:
                case DatabaseType.MongoDB:
                    var context = GetContextDirectory(databaseType);
                    await _dockerComposeService.DownAsync(context);
                    break;
                case DatabaseType.SQLite:
                case DatabaseType.Known:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}