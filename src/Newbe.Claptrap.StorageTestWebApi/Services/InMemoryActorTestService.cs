using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.TestSuit.QuickSetupTools;

namespace Newbe.Claptrap.StorageTestWebApi.Services
{
    public class InMemoryActorTestService : IInMemoryActorTestService
    {
        private readonly ILogger<InMemoryActorTestService> _logger;
        private readonly IOptions<TestConsoleOptions> _options;
        private readonly IDataBaseService _dataBaseService;
        private readonly ClaptrapFactory _claptrapFactory;
        private readonly int _actorCount;


        public InMemoryActorTestService(
            ILogger<InMemoryActorTestService> logger,
            IOptions<TestConsoleOptions> options,
            IDataBaseService dataBaseService,
            IClaptrapFactory claptrapFactory)
        {
            _logger = logger;
            _options = options;
            _dataBaseService = dataBaseService;
            _claptrapFactory = (ClaptrapFactory) claptrapFactory;
            _actorCount = _options.Value.ActorCount;
        }

        private IEventSaver[] accounts;
        private ILifetimeScope[] _scopes;
        private UnitEvent[] events;
        private int[] versions;

        public async Task InitAsync()
        {
            _logger.LogInformation("Start to init async");
            if (_options.Value.SetupLocalDatabase)
            {
                var databaseType = _options.Value.DatabaseType;
                await _dataBaseService.StartAsync(databaseType, 30);
                _logger.LogInformation("Database setup completed.");
            }

            var optionsValue = _options.Value;
            accounts = new IEventSaver[optionsValue.ActorCount];
            _scopes = new ILifetimeScope[optionsValue.ActorCount];
            var scopes = Enumerable.Range(0, optionsValue.ActorCount)
                .Select((i, x) =>
                {
                    var re = new
                    {
                        Scope = _claptrapFactory.BuildClaptrapLifetimeScope(new ClaptrapIdentity(x.ToString(),
                            Codes.Account)),
                        ClaptrapIdentity = new ClaptrapIdentity(x.ToString(),
                            Codes.Account),
                    };
                    _scopes[i] = re.Scope;
                    return re;
                })
                .ToArray();

            _logger.LogInformation("Scopes created.");
            for (var i = 0; i < optionsValue.ActorCount; i++)
            {
                accounts[i] = scopes[i].Scope.Resolve<IEventSaver>();
            }

            _logger.LogInformation("Accounts created.");
            events = new UnitEvent[optionsValue.ActorCount];
            for (var i = 0; i < optionsValue.ActorCount; i++)
            {
                events[i] = UnitEvent.Create(scopes[i].ClaptrapIdentity);
            }

            _logger.LogInformation("Events created.");
            versions = new int[optionsValue.ActorCount];

            var id = new ClaptrapIdentity("1", Codes.Account);
            await using var scope = _claptrapFactory.BuildClaptrapLifetimeScope(id);
            var eventSaverMigration = scope.Resolve<IEventSaverMigration>();
            await eventSaverMigration.MigrateAsync();
            _logger.LogInformation("Database migration done.");
        }

        private int _counter;
        private int _userIndex;

        public async Task<int> RunAsync()
        {
            var uid = Interlocked.Increment(ref _userIndex) % _actorCount;
            var eventSaver = accounts[uid];
            var e = events[uid];
            var version = Interlocked.Increment(ref versions[uid]);
            await eventSaver.SaveEventAsync(e with{Version = version});
            var re = Interlocked.Increment(ref _counter);
            return re;
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