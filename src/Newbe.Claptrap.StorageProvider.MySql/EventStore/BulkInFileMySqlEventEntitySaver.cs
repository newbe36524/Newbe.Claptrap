using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using MySql.Data.MySqlClient;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.EventStore
{
    /// <summary>
    /// It seem nothing better than <see cref="InsertValuesMySqlEventEntitySaver"/>
    /// </summary>
    public class BulkInFileMySqlEventEntitySaver : IEventEntitySaver<EventEntity>
    {
        private readonly IDbFactory _dbFactory;
        private readonly IBatchOperator<EventEntity> _batchOperator;
        private readonly string _eventTableName;
        private readonly string _connectionName;
        private readonly string _schemaName;
        private readonly CsvConfiguration _csvConfiguration;

        private int _fileIndex = 0;

        public BulkInFileMySqlEventEntitySaver(
            ConcurrentListBatchOperator<EventEntity>.Factory batchOperatorFactory,
            IClaptrapIdentity identity,
            IDbFactory dbFactory,
            IMySqlEventStoreOptions options,
            IBatchOperatorContainer batchOperatorContainer)
        {
            _dbFactory = dbFactory;
            var locator = options.RelationalEventStoreLocator;
            _connectionName = locator.GetConnectionName(identity);
            _schemaName = locator.GetSchemaName(identity);
            _eventTableName = locator.GetEventTableName(identity);
            var operatorKey = new BatchOperatorKey()
                .With(nameof(BulkInFileMySqlEventEntitySaver))
                .With(_connectionName)
                .With(_schemaName)
                .With(_eventTableName);
            _batchOperator = (IBatchOperator<EventEntity>) batchOperatorContainer.GetOrAdd(
                operatorKey, () => batchOperatorFactory.Invoke(
                    new BatchOperatorOptions<EventEntity>(options)
                    {
                        DoManyFunc = (entities, cacheData) => SaveManyAsync(entities),
                        DoManyFuncName = $"event batch saver for {operatorKey.AsStringKey()}"
                    }));
            _csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = "┣",
                CultureInfo = CultureInfo.InvariantCulture,
                HasHeaderRecord = false,
                ShouldQuote = (s, context) => false,
            };
            _csvConfiguration.TypeConverterOptionsCache.AddOptions<DateTime>(new TypeConverterOptions
            {
                Formats = new[] {"u"}
            });
        }

        private int GetCommandHashCode(int count)
        {
            return HashCode.Combine(_connectionName, _schemaName, _eventTableName, count);
        }

        public Task SaveAsync(EventEntity entity)
        {
            var valueTask = _batchOperator.CreateTask(entity);
            if (valueTask.IsCompleted)
            {
                return Task.CompletedTask;
            }

            return valueTask.AsTask();
        }

        private static readonly string[] EntityNames =
            typeof(RelationalEventEntity).GetProperties().Select(x => x.Name).ToArray();

        public async Task SaveManyAsync(IEnumerable<EventEntity> entities)
        {
            var array = entities as EventEntity[] ?? entities.ToArray();
            var items = array
                .Select(x => new RelationalEventEntity
                {
                    claptrap_id = x.ClaptrapId,
                    claptrap_type_code = x.ClaptrapTypeCode,
                    created_time = x.CreatedTime,
                    event_data = x.EventData,
                    event_type_code = x.EventTypeCode,
                    version = x.Version
                })
                .OrderBy(x => x.claptrap_type_code)
                .ThenBy(x => x.claptrap_id)
                .ThenBy(x => x.version)
                .ToArray();

            await using var db = _dbFactory.GetConnection(_connectionName);
            var newIndex = Interlocked.Increment(ref _fileIndex);
            var filename = $"{GetCommandHashCode(0)}_{newIndex}.csv";

            var sw = Stopwatch.StartNew();
            var targetFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
            await using (var fileStream = new FileStream(targetFile, FileMode.CreateNew))
            {
                var streamWriter = new StreamWriter(fileStream, new UTF8Encoding(false), 4096)
                {
                    AutoFlush = true
                };
                var csvSerializer = new CsvSerializer(streamWriter, _csvConfiguration, true);
                var csvWriter = new CsvWriter(csvSerializer);
                await csvWriter.WriteRecordsAsync(items);
                await csvWriter.FlushAsync();
            }

            Console.WriteLine($"{filename} cost {sw.ElapsedMilliseconds} ms");
            sw = Stopwatch.StartNew();
            try
            {
                var mySqlBulkLoader = new MySqlBulkLoader(db)
                {
                    Local = true,
                    Connection = db,
                    ConflictOption = MySqlBulkLoaderConflictOption.None,
                    TableName = _eventTableName,
                    FileName = targetFile,
                    FieldTerminator = "┣",
                };
                mySqlBulkLoader.Columns.AddRange(EntityNames);
                await mySqlBulkLoader.LoadAsync();
            }
            finally
            {
                Console.WriteLine($"submit {filename} cost {sw.ElapsedMilliseconds} ms");
                if (File.Exists(targetFile))
                {
                    File.Delete(targetFile);
                }
            }
        }
    }
}