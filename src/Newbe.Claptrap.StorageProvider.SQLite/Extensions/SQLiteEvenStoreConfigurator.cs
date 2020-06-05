using System;
using System.IO;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Extensions
{
    public class SQLiteEvenStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public SQLiteEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private SQLiteEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public SQLiteEvenStoreConfigurator SharedTable()
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteEventStoreOptions
                {
                    RelationalEventStoreLocator = new RelationalEventStoreLocator(
                        schemaName: "main",
                        connectionName: "shared/claptrap.events.db",
                        eventTableName: "events"),
                };
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }

        public SQLiteEvenStoreConfigurator OneIdOneFile()
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteEventStoreOptions
                {
                    RelationalEventStoreLocator = new RelationalEventStoreLocator(
                        schemaName: "main",
                        connectionNameFunc: masterOrSelfIdentity =>
                            Path.Combine($"{masterOrSelfIdentity.TypeCode}_{masterOrSelfIdentity.Id}", "eventDb.db"),
                        eventTableName: "events"),
                };
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }

        public SQLiteEvenStoreConfigurator OneTypeOneFile()
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteEventStoreOptions
                {
                    RelationalEventStoreLocator = new RelationalEventStoreLocator(
                        schemaName: "main",
                        connectionNameFunc: masterOrSelfIdentity =>
                            Path.Combine(masterOrSelfIdentity.TypeCode, "eventDb.db"),
                        eventTableName: "events"),
                };
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }


        public SQLiteEvenStoreConfigurator CustomLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? eventTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? eventTableNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new SQLiteEventStoreOptions
                {
                    RelationalEventStoreLocator = new RelationalEventStoreLocator(
                        schemaName,
                        connectionName,
                        eventTableName,
                        schemaNameFunc,
                        connectionNameFunc,
                        eventTableNameFunc),
                };
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
            return this;
        }
    }
}