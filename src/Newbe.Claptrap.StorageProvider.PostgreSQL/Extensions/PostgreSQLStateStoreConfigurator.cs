using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions
{
    public class PostgreSQLStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public PostgreSQLStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private PostgreSQLStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public PostgreSQLStateStoreConfigurator SharedTable(Action<PostgreSQLStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateTableName = Defaults.StateTableName,
                }, action);

        public PostgreSQLStateStoreConfigurator OneIdOneTable(Action<PostgreSQLStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateTableNameFunc = id => $"{id.TypeCode}_{id.Id}_{Defaults.StateTableName}",
                }, action);


        public PostgreSQLStateStoreConfigurator OneTypeOneTable(Action<PostgreSQLStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateTableNameFunc = id => $"{id.TypeCode}_{Defaults.StateTableName}",
                }, action);

        private PostgreSQLStateStoreConfigurator UseLocator(
            IRelationalStateStoreLocator relationalEventStoreLocator,
            Action<PostgreSQLStateStoreOptions>? action = null
        ) =>
            ConfigureOptions(providerOptions =>
            {
                var stateOptions = new PostgreSQLStateStoreOptions
                {
                    RelationalStateStoreLocator = relationalEventStoreLocator,
                };
                action?.Invoke(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
    }
}