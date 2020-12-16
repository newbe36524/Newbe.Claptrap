using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.CapacityBurning.Grains;
using Newbe.Claptrap.StorageSetup;

namespace Newbe.Claptrap.CapacityBurning.Services
{
    public class EventSavingBurningService : IBurningService
    {
        public delegate EventSavingBurningService Factory(EventSavingBurningOptions options);

        private readonly EventSavingBurningOptions _options;
        private readonly IOptions<BurningDatabaseOptions> _burningDatabaseOptions;
        private readonly ILogger<EventSavingBurningService> _logger;
        private readonly IDataBaseService _dataBaseService;
        private readonly ClaptrapFactory _claptrapFactory;
        private (ClaptrapIdentity id, ILifetimeScope lifetimeScope, IEventSaver saver)[] _savers;

        public EventSavingBurningService(
            EventSavingBurningOptions options,
            IOptions<BurningDatabaseOptions> burningDatabaseOptions,
            ILogger<EventSavingBurningService> logger,
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

        public Task StartAsync()
        {
            Parallel.ForEach(Enumerable.Range(0, _options.BatchCount)
                .SelectMany(i => Enumerable.Range(_options.BatchSize * i, _options.BatchSize))
                .SelectMany(version =>
                {
                    return _savers.Select(eventSaver =>
                    {
                        var (id, _, saver) = eventSaver;
                        return (saver, unitEvent: new UnitEvent(id,
                            Codes.BurningEvent,
                            UnitEvent.UnitEventData.Create())
                        {
                            Version = version
                        });
                    });
                }), t =>
            {
                var (eventSaver1, unitEvent) = t;
                eventSaver1.SaveEventAsync(unitEvent);
            });

            return Task.CompletedTask;
        }

        [Time]
        private static (ClaptrapIdentity id, ILifetimeScope lifetimeScope, IEventSaver saver)[] CreateSavers(
            IEnumerable<(ClaptrapIdentity id, ILifetimeScope lifetimeScope)> lifetimeScopes)
        {
            var eventSavers = lifetimeScopes
                .Select(x => (x.id,
                    x.lifetimeScope,
                    saver: x.lifetimeScope.Resolve<IEventSaver>()))
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