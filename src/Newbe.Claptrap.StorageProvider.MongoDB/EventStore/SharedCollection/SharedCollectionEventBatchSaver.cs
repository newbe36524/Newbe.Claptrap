using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MongoDB.EventStore.SharedCollection
{
    public class SharedCollectionEventBatchSaver : ISharedCollectionEventBatchSaver
    {
        public delegate SharedCollectionEventBatchSaver Factory(string connectionName,
            string databaseName,
            string collectionName);

        private readonly string _connectionName;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IDbFactory _dbFactory;
        private readonly ILogger<SharedCollectionEventBatchSaver> _logger;
        private readonly Subject<SavingItem> _subject = new Subject<SavingItem>();

        public SharedCollectionEventBatchSaver(
            string connectionName,
            string databaseName,
            string collectionName,
            IDbFactory dbFactory,
            ILogger<SharedCollectionEventBatchSaver> logger)
        {
            _connectionName = connectionName;
            _databaseName = databaseName;
            _collectionName = collectionName;
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
                .Select(x => new SharedCollectionEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .ToArray();

            var client = _dbFactory.GetConnection(_connectionName);
            var db = client.GetDatabase(_databaseName);
            var collection = db.GetCollection<SharedCollectionEventEntity>(_collectionName);
            var insertOneModels = items.Select(x => new InsertOneModel<SharedCollectionEventEntity>(x));
            await collection.BulkWriteAsync(insertOneModels);
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