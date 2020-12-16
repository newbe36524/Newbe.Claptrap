using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageSetup;
using Newbe.Claptrap.TestSuit.QuickSetupTools;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public class EventInsertTestService : IEventInsertTestService
    {
        private readonly IDataBaseService _dataBaseService;
        private readonly ClaptrapFactory _claptrapFactory;

        public EventInsertTestService(
            IDataBaseService dataBaseService,
            IClaptrapFactory claptrapFactory)
        {
            _dataBaseService = dataBaseService;
            _claptrapFactory = (ClaptrapFactory) claptrapFactory;
        }

        public async Task RunAsync()
        {
            var databaseType = DatabaseType.MongoDB;
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
            // await eventSaverMigration.MigrateAsync();

            var saver = scope.Resolve<IEventEntitySaver<EventEntity>>();
            var mapper = scope.Resolve<IEventEntityMapper<EventEntity>>();
            var totalCount = 5_000_000;
            var batchSize = 10;
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
                Console.WriteLine(sw.ElapsedMilliseconds);
            }

            Console.WriteLine($@"total: {timeList.Sum()}");
        }
    }
}