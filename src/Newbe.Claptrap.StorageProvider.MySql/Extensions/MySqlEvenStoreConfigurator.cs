using System;
using Newbe.Claptrap.StorageProvider.MySql.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.MySql.Extensions
{
    public class MySqlEvenStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public MySqlEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        public MySqlEvenStoreConfigurator SharedTable(Action<MySqlEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventTableName = Defaults.EventTableName
                }, action);

        public MySqlEvenStoreConfigurator OneIdOneTable(Action<MySqlEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventTableName = $"[TypeCode]_[Id]_{Defaults.EventTableName}",
                }, action);

        public MySqlEvenStoreConfigurator OneTypeOneTable(Action<MySqlEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventTableName = $"[TypeCode]_{Defaults.EventTableName}",
                }, action);

        private MySqlEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public MySqlEvenStoreConfigurator UseLocator(
            IRelationalEventStoreLocator relationalEventStoreLocator,
            Action<MySqlEventStoreOptions>? action = null) =>
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new MySqlEventStoreOptions
                {
                    RelationalEventStoreLocator = relationalEventStoreLocator,
                };
                action?.Invoke(eventOptions);
                providerOptions.EventLoaderOptions = eventOptions;
                providerOptions.EventSaverOptions = eventOptions;
            });
    }
}