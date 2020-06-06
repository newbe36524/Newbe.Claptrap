using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Extensions
{
    public class MySqlStateStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public MySqlStateStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }


        public MySqlStateStoreConfigurator SharedTable(Action<MySqlStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateTableName = Defaults.StateTableName,
                }, action);

        public MySqlStateStoreConfigurator OneIdOneTable(Action<MySqlStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateTableNameFunc = id => $"{id.TypeCode}_{id.Id}_{Defaults.StateTableName}",
                }, action);


        public MySqlStateStoreConfigurator OneTypeOneTable(Action<MySqlStateStoreOptions>? action = null)
            =>
                UseLocator(new RelationalStateStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    StateTableNameFunc = id => $"{id.TypeCode}_{Defaults.StateTableName}",
                }, action);

        private MySqlStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        private MySqlStateStoreConfigurator UseLocator(
            IRelationalStateStoreLocator relationalEventStoreLocator,
            Action<MySqlStateStoreOptions>? action = null
        )
            => ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MySqlStateStoreOptions
                {
                    RelationalStateStoreLocator = relationalEventStoreLocator,
                };
                action?.Invoke(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
    }
}