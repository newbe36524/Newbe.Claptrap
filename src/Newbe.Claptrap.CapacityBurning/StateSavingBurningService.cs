using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.CapacityBurning.Grains;

namespace Newbe.Claptrap.CapacityBurning
{
    public class StateSavingBurningService : IBurningService
    {
        public delegate StateSavingBurningService Factory(StateSavingBurningOptions options);

        private readonly StateSavingBurningOptions _options;
        private readonly ILogger<StateSavingBurningService> _logger;
        private readonly ClaptrapFactory _claptrapFactory;

        public StateSavingBurningService(
            StateSavingBurningOptions options,
            ILogger<StateSavingBurningService> logger,
            ClaptrapFactory claptrapFactory)
        {
            _options = options;
            _logger = logger;
            _claptrapFactory = claptrapFactory;
        }

        public async Task StartAsync()
        {
            _logger.LogInformation(" start {name} with options {@options}",
                nameof(EventSavingBurningService),
                _options);

            var lifetimeScopes = CreateLifetimeScopes();
            var eventSavers = CreateEventSavers(lifetimeScopes);

            for (var i = 0; i < _options.BatchCount; i++)
            {
                var fromVersion = _options.BatchSize * i;
                await RunOneBatch(eventSavers, fromVersion, _options.BatchSize);
            }
        }

        [Time]
        private static async Task RunOneBatch(
            IEnumerable<(ClaptrapIdentity id, ILifetimeScope lifetimeScope, IStateSaver saver)> eventSavers,
            int fromVersion, int batchSize)
        {
            var tasks = eventSavers
                .SelectMany(x =>
                    Enumerable.Range(fromVersion, batchSize)
                        .Select(version => x.saver.SaveAsync(new UnitState
                        {
                            Data = new UnitState.UnitStateData(),
                            Identity = x.id,
                            Version = version
                        }))
                )
                .ToArray();
            await Task.WhenAll(tasks);
        }

        [Time]
        private static (ClaptrapIdentity id, ILifetimeScope lifetimeScope, IStateSaver saver)[] CreateEventSavers(
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