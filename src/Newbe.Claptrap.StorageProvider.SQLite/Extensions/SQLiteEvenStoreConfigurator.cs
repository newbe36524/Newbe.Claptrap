using System;
using System.IO;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Extensions
{
    public class SQLiteEvenStoreConfigurator
    {
        public const string SQLiteSchemaName = "main";
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public SQLiteEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }


        public SQLiteEvenStoreConfigurator SharedTable(Action<SQLiteEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = SQLiteSchemaName,
                    ConnectionName = "shared/claptrap.events.db",
                    EventTableName = Defaults.EventTableName
                }, action);


        public SQLiteEvenStoreConfigurator OneIdOneFile(Action<SQLiteEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = SQLiteSchemaName,
                    ConnectionNameFunc = identity =>
                        Path.Combine($"{identity.TypeCode}_{identity.Id}", "eventDb.db"),
                    EventTableName = Defaults.EventTableName
                }, action);


        public SQLiteEvenStoreConfigurator OneTypeOneFile(Action<SQLiteEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = SQLiteSchemaName,
                    ConnectionNameFunc = identity =>
                        Path.Combine(identity.TypeCode, "eventDb.db"),
                    EventTableName = Defaults.EventTableName
                }, action);

        private SQLiteEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        private SQLiteEvenStoreConfigurator UseLocator(
            IRelationalEventStoreLocator relationalEventStoreLocator,
            Action<SQLiteEventStoreOptions>? action = null
        )
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteEventStoreOptions
                {
                    RelationalEventStoreLocator = relationalEventStoreLocator,
                };
                action?.Invoke(eventOptions);
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }
    }
}