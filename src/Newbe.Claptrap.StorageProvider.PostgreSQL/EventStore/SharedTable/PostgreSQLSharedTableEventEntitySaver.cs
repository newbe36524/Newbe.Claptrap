using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.Tools;
using Npgsql;
using NpgsqlTypes;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.EventStore.SharedTable
{
    public class PostgreSQLSharedTableEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IBatchOperator<EventEntity> _batchOperator;

        public PostgreSQLSharedTableEventEntitySaver(
            BatchOperator<EventEntity>.Factory batchOperatorFactory,
            IDbFactory dbFactory,
            IPostgreSQLSharedTableEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer)
        {
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                new SharedTableEventBatchOperatorKey(options), () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>
                    {
                        BufferCount = options.InsertManyWindowCount,
                        BufferTime = options.InsertManyWindowTimeInMilliseconds.HasValue
                            ? TimeSpan.FromMilliseconds(options.InsertManyWindowTimeInMilliseconds.Value)
                            : default,
                        DoManyFunc = entities => SaveManyCoreMany(dbFactory, options, entities)
                    }));
        }

        public Task SaveAsync(EventEntity entity)
        {
            return _batchOperator.CreateTask(entity);
        }

        private readonly struct SharedTableEventBatchOperatorKey : IBatchOperatorKey
        {
            private readonly IPostgreSQLSharedTableEventStoreOptions _options;

            public SharedTableEventBatchOperatorKey(
                IPostgreSQLSharedTableEventStoreOptions options)
            {
                _options = options;
            }

            public string AsStringKey()
            {
                return
                    $"{nameof(PostgreSQLSharedTableEventEntitySaver)}-{_options.ConnectionName}-{_options.SchemaName}-{_options.EventTableName}";
            }
        }

        private static async Task SaveManyCoreMany(
            IDbFactory factory,
            IPostgreSQLSharedTableEventStoreOptions options,
            IEnumerable<EventEntity> entities)
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

            await using var db = (NpgsqlConnection) factory.GetConnection(options.ConnectionName);
            await db.OpenAsync();
            using var importer =
                db.BeginBinaryImport(
                    $"COPY {options.SchemaName}.{options.EventTableName} (claptrap_type_code, claptrap_id, version, event_type_code, event_data, created_time) FROM STDIN (FORMAT BINARY)");
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
    }
}