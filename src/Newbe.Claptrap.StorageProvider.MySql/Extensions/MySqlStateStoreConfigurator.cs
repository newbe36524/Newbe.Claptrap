using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;
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
        {
            return UseLocator(new RelationalStateStoreLocator
            {
                SchemaName = Defaults.SchemaName,
                ConnectionName = Defaults.ConnectionName,
                StateTableName = Defaults.StateTableName
            }, action);
        }

        public MySqlStateStoreConfigurator OneIdOneTable(Action<MySqlStateStoreOptions>? action = null)
        {
            return UseLocator(new RelationalStateStoreLocator
            {
                SchemaName = Defaults.SchemaName,
                ConnectionName = Defaults.ConnectionName,
                StateTableName = $"[TypeCode]_[Id]_{Defaults.StateTableName}"
            }, action);
        }


        public MySqlStateStoreConfigurator OneTypeOneTable(Action<MySqlStateStoreOptions>? action = null)
        {
            return UseLocator(new RelationalStateStoreLocator
            {
                SchemaName = Defaults.SchemaName,
                ConnectionName = Defaults.ConnectionName,
                StateTableName = $"[TypeCode]_{Defaults.StateTableName}"
            }, action);
        }

        public MySqlStateStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MySqlStateStoreConfigurator UseLocator(
            IRelationalStateStoreLocator relationalEventStoreLocator,
            Action<MySqlStateStoreOptions>? action = null
        )
        {
            return ConfigureOptions(providerOptions =>
            {
                var stateOptions = new MySqlStateStoreOptions
                {
                    RelationalStateStoreLocator = relationalEventStoreLocator
                };
                action?.Invoke(stateOptions);
                providerOptions.StateLoaderOptions = stateOptions;
                providerOptions.StateSaverOptions = stateOptions;
            });
        }
    }
}