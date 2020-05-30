using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Dapper;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Npgsql;
using NpgsqlTypes;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
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
        private readonly Subject<SavingItem> _subject = new Subject<SavingItem>();

        public SharedTableEventBatchSaver(
            string dbName,
            string schemaName,
            string eventTableName,
            IDbFactory dbFactory)
        {
            _dbName = dbName;
            _schemaName = schemaName;
            _eventTableName = eventTableName;
            _dbFactory = dbFactory;
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

            await using var db = (NpgsqlConnection) _dbFactory.GetConnection(_dbName);
            await db.OpenAsync();
            using var importer =
                db.BeginBinaryImport(
                    $"COPY {_schemaName}.{_eventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) FROM STDIN (FORMAT BINARY)");
            foreach (var entity in items)
            {
                importer.StartRow();
                importer.Write(entity.claptrap_type_code);
                importer.Write(entity.claptrap_id);
                importer.Write(entity.version, NpgsqlDbType.Bigint);
                importer.Write(entity.event_type_code);
                importer.Write(entity.event_data);
                importer.Write(entity.created_time, NpgsqlDbType.Date);
            }

            importer.Complete();
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

        private struct SavingItem
        {
            public EventEntity EventEntity { get; set; }
            public TaskCompletionSource<int> Tcs { get; set; }
        }
    }
}