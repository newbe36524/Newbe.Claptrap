using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.TestSuit.QuickSetupTools;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public class EventSavingTestService : IEventSavingTestService
    {
        private readonly ILogger<EventSavingTestService> _logger;
        private readonly IOptions<TestConsoleOptions> _options;
        private readonly IDataBaseService _dataBaseService;
        private readonly IReportFormat<SavingEventResult> _reportFormat;
        private readonly ClaptrapFactory _claptrapFactory;

        public EventSavingTestService(
            ILogger<EventSavingTestService> logger,
            IOptions<TestConsoleOptions> options,
            IDataBaseService dataBaseService,
            IReportFormat<SavingEventResult> reportFormat,
            IClaptrapFactory claptrapFactory)
        {
            _logger = logger;
            _options = options;
            _dataBaseService = dataBaseService;
            _reportFormat = reportFormat;
            _claptrapFactory = (ClaptrapFactory) claptrapFactory;
        }

        public async Task RunAsync()
        {
            var databaseType = _options.Value.DatabaseType;
            await _dataBaseService.StartAsync(databaseType, 30);
            try
            {
                await RunCoreAsync();
            }
            finally
            {
                await _dataBaseService.CleanAsync(databaseType);
            }
        }

        private async Task RunCoreAsync()
        {
            var id = new ClaptrapIdentity("1", Codes.Account);
            await using var scope = _claptrapFactory.BuildClaptrapLifetimeScope(id);
            var eventSaverMigration = scope.Resolve<IEventSaverMigration>();
            await eventSaverMigration.MigrateAsync();

            var saver = scope.Resolve<IEventEntitySaver<EventEntity>>();
            var mapper = scope.Resolve<IEventEntityMapper<EventEntity>>();
            var totalCount = _options.Value.TotalCount;
            var batchSize = _options.Value.BatchSize;
            var batchCount = totalCount / batchSize;
            var timeList = new List<long>();
            for (int i = 0; i < batchCount; i++)
            {
                var versionStart = i * batchSize;
                var events = Enumerable.Range(versionStart, batchSize)
                    .Select(version => new UnitEvent(id, Codes.AccountBalanceMinion, UnitEvent.UnitEventData.Create())
                    {
                        Version = version
                    })
                    .Select(mapper.Map);

                var sw = Stopwatch.StartNew();
                await saver.SaveManyAsync(events);
                sw.Stop();
                timeList.Add(sw.ElapsedMilliseconds);
                _logger.LogTrace("batch {i} {percent:00.00}%: {total}", i, i * 1.0 / batchCount * 100,
                    sw.Elapsed.Humanize());
            }

            var result = new SavingEventResult
            {
                TotalCount = totalCount,
                BatchCount = batchCount,
                BatchSize = batchSize,
                BatchTimes = timeList
            };

            var report = await _reportFormat.FormatAsync(result);
            _logger.LogInformation(report);
        }
    }
}