using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Autofac;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.CapacityBurning.Grains;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public class StateSavingBurningService : IBurningService
    {
        public delegate StateSavingBurningService Factory(StateSavingBurningOptions options);

        private readonly StateSavingBurningOptions _options;
        private readonly IOptions<BurningDatabaseOptions> _burningDatabaseOptions;
        private readonly ILogger<StateSavingBurningService> _logger;
        private readonly IDataBaseService _dataBaseService;
        private readonly ClaptrapFactory _claptrapFactory;
        private (ClaptrapIdentity id, ILifetimeScope lifetimeScope, IStateSaver saver)[] _savers;

        public StateSavingBurningService(
            StateSavingBurningOptions options,
            IOptions<BurningDatabaseOptions> burningDatabaseOptions,
            ILogger<StateSavingBurningService> logger,
            IDataBaseService dataBaseService,
            ClaptrapFactory claptrapFactory)
        {
            _options = options;
            _burningDatabaseOptions = burningDatabaseOptions;
            _logger = logger;
            _dataBaseService = dataBaseService;
            _claptrapFactory = claptrapFactory;
        }

        public async Task PrepareAsync()
        {
            _logger.LogInformation(" start {name} with options {@options}",
                nameof(EventSavingBurningService),
                _options);
            await _dataBaseService.StartAsync(_burningDatabaseOptions.Value.DatabaseType, _options.PreparingSleepInSec);

            var lifetimeScopes = CreateLifetimeScopes();
            var savers = CreateSavers(lifetimeScopes);
            _savers = savers;
        }

        public async Task CleanAsync()
        {
            await _dataBaseService.CleanAsync(_burningDatabaseOptions.Value.DatabaseType);
        }

        public async Task StartAsync()
        {
            var task = Enumerable.Range(0, _options.RepeatCount)
                .ToObservable()
                .SelectMany(i => Enumerable.Range(_options.VersionRange * i, _options.VersionRange))
                .SelectMany(version =>
                {
                    return _savers.Select(eventSaver =>
                    {
                        var (id, _, saver) = eventSaver;
                        return Observable.FromAsync(() => saver.SaveAsync(new UnitState
                        {
                            Data = UnitState.UnitStateData.Create(),
                            Identity = id,
                            Version = version
                        }));
                    });
                })
                .Merge(_options.ConcurrentCount)
                .ToTask();
            await task;
        }

        [Time]
        private static (ClaptrapIdentity id, ILifetimeScope lifetimeScope, IStateSaver saver)[] CreateSavers(
            IEnumerable<(ClaptrapIdentity id, ILifetimeScope lifetimeScope)> lifetimeScopes)
        {
            var eventSavers = lifetimeScopes
                .Select(x => (x.id,
                    x.lifetimeScope,
                    saver: x.lifetimeScope.Resolve<IStateSaver>()))
                .ToArray();
            return eventSavers;
        }

        [Time]
        private IEnumerable<(ClaptrapIdentity id, ILifetimeScope lifetimeScope)> CreateLifetimeScopes()
        {
            var lifetimeScopes = Enumerable
                .Range(0, _options.UserIdCount)
                .Select(id => new ClaptrapIdentity(id.ToString(), Codes.Burning))
                .Select(id =>
                    (id, lifetimeScope: _claptrapFactory.BuildClaptrapLifetimeScope(id)))
                .ToArray();
            return lifetimeScopes;
        }
    }
}