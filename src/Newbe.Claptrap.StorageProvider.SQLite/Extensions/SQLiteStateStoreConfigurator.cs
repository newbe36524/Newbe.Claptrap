using System;
using System.IO;
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

        private SQLiteStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public SQLiteStateStoreConfigurator SharedTable()
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new IsqLiteStateStoreOptions
                {
                    RelationalStateStoreLocator = new RelationalStateStoreLocator(
                        schemaName: "main",
                        connectionName: "shared/states.db",
                        stateTableName: "states"),
                };
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });

            return this;
        }

        public SQLiteStateStoreConfigurator OneIdOneFile()
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new IsqLiteStateStoreOptions
                {
                    RelationalStateStoreLocator = new RelationalStateStoreLocator(
                        schemaName: "main",
                        connectionNameFunc: identity =>
                            Path.Combine($"{identity.TypeCode}_{identity.Id}", "stateDb.db"),
                        stateTableName: "states"),
                };
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
            return this;
        }


        public SQLiteStateStoreConfigurator OneTypeOneFile()
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new IsqLiteStateStoreOptions
                {
                    RelationalStateStoreLocator = new RelationalStateStoreLocator(
                        schemaName: "main",
                        connectionNameFunc: identity => Path.Combine($"{identity.TypeCode}", "stateDb.db"),
                        stateTableName: "states"),
                };
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });

            return this;
        }

        public SQLiteStateStoreConfigurator CustomLocator(
            string? schemaName = null,
            string? connectionName = null,
            string? stateTableName = null,
            Func<IClaptrapIdentity, string>? schemaNameFunc = null,
            Func<IClaptrapIdentity, string>? connectionNameFunc = null,
            Func<IClaptrapIdentity, string>? stateTableNameFunc = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new IsqLiteStateStoreOptions
                {
                    RelationalStateStoreLocator = new RelationalStateStoreLocator(
                        schemaName,
                        connectionName,
                        stateTableName,
                        schemaNameFunc,
                        connectionNameFunc,
                        stateTableNameFunc),
                };
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateLoaderOptions = stateOptions;
            });
            return this;
        }
    }
}