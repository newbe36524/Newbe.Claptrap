using System;
using System.IO;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;
using Newbe.Claptrap.StorageProvider.SQLite.Options;

namespace Newbe.Claptrap.StorageProvider.SQLite.Extensions
{
    public class SQLiteStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public SQLiteStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }


        public SQLiteStateStoreConfigurator SharedTable(
            Action<SQLiteStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Consts.SQLiteSchemaName,
                    ConnectionName = "shared/states.db",
                    StateTableName = Defaults.StateTableName,
                }, action);

        public SQLiteStateStoreConfigurator OneIdOneFile(
            Action<SQLiteStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Consts.SQLiteSchemaName,
                    ConnectionNameFunc = identity => Path.Combine($"{identity.TypeCode}_{identity.Id}", "stateDb.db"),
                    StateTableName = Defaults.StateTableName,
                }, action);


        public SQLiteStateStoreConfigurator OneTypeOneFile(
            Action<SQLiteStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Consts.SQLiteSchemaName,
                    ConnectionNameFunc = identity => Path.Combine($"{identity.TypeCode}", "stateDb.db"),
                    StateTableName = Defaults.StateTableName,
                }, action);

        private SQLiteStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        private SQLiteStateStoreConfigurator UseLocator(
            IRelationalStateStoreLocator relationalEventStoreLocator,
            Action<SQLiteStateStoreOptions>? action = null
        )
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new SQLiteStateStoreOptions
                {
                    RelationalStateStoreLocator = relationalEventStoreLocator,
                };
                action?.Invoke(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
            return this;
        }
    }
}