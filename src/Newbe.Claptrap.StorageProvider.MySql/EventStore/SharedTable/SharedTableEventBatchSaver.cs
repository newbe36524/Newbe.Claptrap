using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Extensions.Logging;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore.SharedTable
{
    public class SharedTableEventBatchSaver : ISharedTableEventBatchSaver
    {
        public delegate SharedTableEventBatchSaver Factory(string dbName,
            string schemaName,
            string eventTableName);

        private readonly string _dbName;
        private readonly string _schemaName;
        private readonly string _eventTableName;
        private readonly IDbFactory _dbFactory;
        private readonly ILogger<SharedTableEventBatchSaver> _logger;
        private readonly Subject<SavingItem> _subject = new Subject<SavingItem>();

        public SharedTableEventBatchSaver(
            string dbName,
            string schemaName,
            string eventTableName,
            IDbFactory dbFactory,
            ILogger<SharedTableEventBatchSaver> logger)
        {
            _dbName = dbName;
            _schemaName = schemaName;
            _eventTableName = eventTableName;
            _dbFactory = dbFactory;
            _logger = logger;
            _subject
                .Buffer(TimeSpan.FromMilliseconds(10), 1000)
                .Where(x => x.Count > 0)
                .Select(x => Observable.FromAsync(async () =>
                {
                    try
                    {
                        await SaveManyCoreMany(x.Select(a => a.EventEntity)).ConfigureAwait(false);
                        foreach (var savingItem in x)
                        {
                            savingItem.Tcs.SetResult(0);
                        }
                    }
                    catch (Exception e)
                    {
                        foreach (var savingItem in x)
                        {
                            savingItem.Tcs.SetException(e);
                        }
                    }
                }))
                .Merge()
                .Subscribe();
        }

        private async Task SaveManyCoreMany(IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var items = array
                .Select(x => new SharedTableEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .ToArray();

            var sql = InitSharedTableInsertManySql(array.Length);
            using var db = _dbFactory.GetConnection(_dbName);
            var ps = new DynamicParameters();
            for (var i = 0; i < array.Length; i++)
            {
                foreach (var (parameterName, valueFunc) in SharedTableEventEntity.ValueFactories())
                {
                    var sharedTableEventEntity = items[i];
                    // TODO cache name
                    ps.Add($"@{parameterName}{i}", valueFunc(sharedTableEventEntity));
                }
            }

            await db.ExecuteAsync(sql, ps);
        }

        public Task SaveAsync(EventEntity entity)
        {
            var savingItem = new SavingItem
            {
                Tcs = new TaskCompletionSource<int>(),
                EventEntity = entity
            };
            _subject.OnNext(savingItem);
            return savingItem.Tcs.Task;
        }

        private string InitSharedTableInsertManySql(int count)
        {
            string insertManySqlHeader =
                $"INSERT INTO {_schemaName}.{_eventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) VALUES ";
            var valuesSql = Enumerable.Range(0, count)
                .Select(x =>
                    ValuePartFactory(SharedTableEventEntity.ParameterNames(), x))
                .ToArray();
            var sb = new StringBuilder(insertManySqlHeader);
            sb.Append(string.Join(",", valuesSql));
            return sb.ToString();
        }

        private static string ValuePartFactory(IEnumerable<string> parameters, int index)
        {
            var values = string.Join(",", parameters.Select(x => $"@{x}{index}"));
            var re = $" ({values}) ";
            return re;
        }

        private struct SavingItem
        {
            public EventEntity EventEntity { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}