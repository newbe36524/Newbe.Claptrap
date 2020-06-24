using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using MethodTimer;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.CapacityBurning.Grains;

namespace Newbe.Claptrap.CapacityBurning
{
    public class EventSavingBurningService : IBurningService
    {
        public delegate EventSavingBurningService Factory(EventSavingBurningOptions options);

        private readonly EventSavingBurningOptions _options;
        private readonly ILogger<EventSavingBurningService> _logger;
        private readonly ClaptrapFactory _claptrapFactory;

        public EventSavingBurningService(
            EventSavingBurningOptions options,
            ILogger<EventSavingBurningService> logger,
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
            IEnumerable<(ClaptrapIdentity id, ILifetimeScope lifetimeScope, IEventSaver saver)> eventSavers,
            int fromVersion, int batchSize)
        {
            var tasks = eventSavers
                .SelectMany(x =>
                    Enumerable.Range(fromVersion, batchSize)
                        .Select(version => x.saver.SaveEventAsync(new UnitEvent(x.id, Codes.BurningEvent,
                            new UnitEvent.UnitEventData())
                        {
                            Version = version
                        }))
                )
                .ToArray();
            await Task.WhenAll(tasks);
        }

        [Time]
        private static (ClaptrapIdentity id, ILifetimeScope lifetimeScope, IEventSaver saver)[] CreateEventSavers(
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