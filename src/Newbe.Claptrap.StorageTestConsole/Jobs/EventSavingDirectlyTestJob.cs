using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Humanizer;
using Humanizer.Localisation;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.StorageTestConsole.Services;
using Newbe.Claptrap.TestSuit.QuickSetupTools;
using Newtonsoft.Json;

namespace Newbe.Claptrap.StorageTestConsole
{
    public class EventSavingDirectlyTestJob : ITestJob
    {
        private readonly ILogger<EventSavingDirectlyTestJob> _logger;
        private readonly IOptions<TestConsoleOptions> _options;
        private readonly IDataBaseService _dataBaseService;
        private readonly IReportFormat<SavingEventResult> _reportFormat;
        private readonly IReportManager _reportManager;
        private readonly ClaptrapFactory _claptrapFactory;

        public EventSavingDirectlyTestJob(
            ILogger<EventSavingDirectlyTestJob> logger,
            IOptions<TestConsoleOptions> options,
            IDataBaseService dataBaseService,
            IReportFormat<SavingEventResult> reportFormat,
            IClaptrapFactory claptrapFactory,
            IReportManager reportManager)
        {
            _logger = logger;
            _options = options;
            _dataBaseService = dataBaseService;
            _reportFormat = reportFormat;
            _reportManager = reportManager;
            _claptrapFactory = (ClaptrapFactory) claptrapFactory;
        }

        public async Task RunAsync()
        {
            await _reportManager.InitAsync();
            if (_options.Value.SetupLocalDatabase)
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
            else
            {
                await RunCoreAsync();
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

            var unitEvent = UnitEvent.Create(id);
            var entity = mapper.Map(unitEvent);

            for (var i = 0; i < batchCount; i++)
            {
                var versionStart = i * batchSize;
                var events = Enumerable.Range(versionStart, batchSize)
                    .Select(version => entity with{Version = version});

                var sw = Stopwatch.StartNew();
                await saver.SaveManyAsync(events);
                sw.Stop();
                timeList.Add(sw.ElapsedMilliseconds);
                _logger.LogTrace("batch {i} {percent:00.00}%: {total}", i, i * 1.0 / batchCount * 100,
                    sw.Elapsed.Humanize(maxUnit: TimeUnit.Millisecond));
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

            var reportName =
                $"{_options.Value.DatabaseType.ToString("G").ToLower()}-event_saving_directly-{result.TotalCount}-{result.BatchSize}.json";
            await using var fileStream = _reportManager.CreateFile(reportName);
            await using var streamWriter = new StreamWriter(fileStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(result, Formatting.Indented));
        }
    }
}