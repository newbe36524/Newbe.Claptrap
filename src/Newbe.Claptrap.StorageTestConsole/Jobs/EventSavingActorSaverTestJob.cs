using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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
    public class EventSavingActorSaverTestJob : ITestJob
    {
        private readonly ILogger<EventSavingActorSaverTestJob> _logger;
        private readonly IOptions<TestConsoleOptions> _options;
        private readonly IDataBaseService _dataBaseService;
        private readonly IReportFormat<SavingEventResult> _reportFormat;
        private readonly IReportManager _reportManager;
        private readonly ClaptrapFactory _claptrapFactory;

        public EventSavingActorSaverTestJob(
            ILogger<EventSavingActorSaverTestJob> logger,
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
            var optionsValue = _options.Value;
            var accounts = new IEventSaver[optionsValue.ActorCount];

            var scopes = Enumerable.Range(0, optionsValue.ActorCount)
                .Select(x => new
                {
                    Scope = _claptrapFactory.BuildClaptrapLifetimeScope(new ClaptrapIdentity(x.ToString(),
                        Codes.Account)),
                    ClaptrapIdentity = new ClaptrapIdentity(x.ToString(),
                        Codes.Account),
                })
                .ToArray();

            for (var i = 0; i < optionsValue.ActorCount; i++)
            {
                accounts[i] = scopes[i].Scope.Resolve<IEventSaver>();
            }

            var events = new UnitEvent[optionsValue.ActorCount];
            for (var i = 0; i < optionsValue.ActorCount; i++)
            {
                events[i] = UnitEvent.Create(scopes[i].ClaptrapIdentity);
            }

            await MigrateDatabaseAsync();

            var totalCount = optionsValue.TotalCount;
            var countPerAccount = totalCount / accounts.Length;
            var batchSize = optionsValue.BatchSize;
            var batchCount = countPerAccount / batchSize;
            var timeList = new List<long>();

            var tcs = new TaskCompletionSource<int>();
            int counter = 0;
            var sw = Stopwatch.StartNew();
            await CreateTasks().Last();
            var totalTaskCount = await tcs.Task;
            sw.Stop();
            timeList.Add(sw.ElapsedMilliseconds);
            _logger.LogTrace("{count} total: {total}", totalTaskCount, sw.Elapsed.Humanize(maxUnit: TimeUnit.Millisecond));

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
                $"{optionsValue.DatabaseType.ToString("G").ToLower()}-event_saving_actor-{result.TotalCount}-{result.BatchSize}.json";
            await using var fileStream = _reportManager.CreateFile(reportName);
            await using var streamWriter = new StreamWriter(fileStream);
            await streamWriter.WriteAsync(JsonConvert.SerializeObject(result, Formatting.Indented));

            IEnumerable<Task> CreateTasks()
            {
                for (var i = 0; i < batchCount; i++)
                {
                    for (var j = 0; j < optionsValue.ActorCount; j++)
                    {
                        var eventSaver = accounts[j];
                        var e = events[j];
                        var versionStart = i * batchSize;
                        for (var k = 0; k < batchSize; k++)
                        {
                            var v = versionStart + k;
                            yield return Task.Run(async () =>
                            {
                                await eventSaver.SaveEventAsync(e with{Version = v});
                                var nowValue = Interlocked.Increment(ref counter);
                                if (nowValue % 10000 == 0)
                                {
                                    _logger.LogTrace("completed {percent:00.00}%, now : {time}",
                                        nowValue * 1.0 / totalCount * 100,
                                        sw.Elapsed.Humanize(maxUnit: TimeUnit.Millisecond));
                                }

                                if (nowValue >= totalCount)
                                {
                                    tcs.SetResult(nowValue);
                                }
                            });
                        }
                    }
                }
            }
        }

        private async Task MigrateDatabaseAsync()
        {
            var id = new ClaptrapIdentity("1", Codes.Account);
            await using var scope = _claptrapFactory.BuildClaptrapLifetimeScope(id);
            var eventSaverMigration = scope.Resolve<IEventSaverMigration>();
            await eventSaverMigration.MigrateAsync();
        }
    }
}