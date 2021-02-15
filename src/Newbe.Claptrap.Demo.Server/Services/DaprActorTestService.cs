using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.StorageSetup;
using static Newbe.Claptrap.Demo.Interfaces.Domain.Account.ClaptrapCodes.AccountCodes;

namespace Newbe.Claptrap.Demo.Server.Services
{
    public class DaprActorTestService : IDaprActorTestService
    {
        private readonly ILogger<DaprActorTestService> _logger;
        private readonly IOptions<TestConsoleOptions> _options;
        private readonly IDataBaseService _dataBaseService;
        private readonly int _actorCount;

        public DaprActorTestService(
            ILogger<DaprActorTestService> logger,
            IOptions<TestConsoleOptions> options,
            IDataBaseService dataBaseService)
        {
            _logger = logger;
            _options = options;
            _dataBaseService = dataBaseService;
            _actorCount = _options.Value.ActorCount;
        }

        public async Task InitAsync()
        {
            _logger.LogInformation("Start to init async");
            if (_options.Value.SetupLocalDatabase)
            {
                var databaseType = _options.Value.DatabaseType;
                await _dataBaseService.StartAsync(databaseType, 30);
                _logger.LogInformation("Database setup completed.");
            }

            _logger.LogInformation("Init done");
        }

        private static int _userIndex;

        public async Task<int> RunAsync()
        {
            var uid = Interlocked.Increment(ref _userIndex) % _actorCount;
            var actorId = new ActorId(uid.ToString());
            var account = ActorProxy.Create<IAccount>(actorId, ClaptrapCode);
            var re = await account.TransferInAsync(1M);
            return (int) re;
        }

        public async Task CleanUpAsync()
        {
            if (_options.Value.SetupLocalDatabase)
            {
                var databaseType = _options.Value.DatabaseType;
                await _dataBaseService.CleanAsync(databaseType);
            }
        }
    }
}