using System.Threading;
using System.Threading.Tasks;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.Demo.Interfaces.Domain.Account;
using Newbe.Claptrap.StorageSetup;

namespace Newbe.Claptrap.Demo.Server.Services
{
    public class OrleansActorTestService : IOrleansActorTestService
    {
        private readonly ILogger<OrleansActorTestService> _logger;
        private readonly IOptions<TestConsoleOptions> _options;
        private readonly IDataBaseService _dataBaseService;
        private readonly int _actorCount;

        public OrleansActorTestService(
            ILogger<OrleansActorTestService> logger,
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

        private int _userIndex;

        public async Task<int> RunAsync()
        {
            var uid = Interlocked.Increment(ref _userIndex) % _actorCount;
            var actorId = new ActorId(uid.ToString());
            var account = ActorProxy.Create<IAccount>(actorId,"AccountGrain");
            // var account = _grainFactory.GetGrain<IAccount>(uid.ToString());
            // var account = _grainFactory.GetGrain<IAccount>(1.ToString());
            var re = await account.TransferIn(1M);
            // var re = await account.GetBalance();
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