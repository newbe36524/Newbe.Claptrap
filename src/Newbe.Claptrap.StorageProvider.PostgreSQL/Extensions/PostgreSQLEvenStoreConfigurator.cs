using System;
using Newbe.Claptrap.StorageProvider.PostgreSQL.Options;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions
{
    public class PostgreSQLEvenStoreConfigurator
    {
        private readonly ClaptrapStorageProviderOptions _claptrapStorageProviderOptions;

        public PostgreSQLEvenStoreConfigurator(
            ClaptrapStorageProviderOptions claptrapStorageProviderOptions)
        {
            _claptrapStorageProviderOptions = claptrapStorageProviderOptions;
        }

        private PostgreSQLEvenStoreConfigurator ConfigureOptions(
            Action<ClaptrapStorageProviderOptions> optionsAction)
        {
            optionsAction(_claptrapStorageProviderOptions);
            return this;
        }

        public PostgreSQLEvenStoreConfigurator SharedTable(Action<PostgreSQLEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventTableName = Defaults.EventTableName
                }, action);

        public PostgreSQLEvenStoreConfigurator OneIdOneTable(Action<PostgreSQLEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventTableName = $"[TypeCode]_[Id]_{Defaults.EventTableName}",
                }, action);

        public PostgreSQLEvenStoreConfigurator OneTypeOneTable(Action<PostgreSQLEventStoreOptions>? action = null)
            =>
                UseLocator(new RelationalEventStoreLocator
                {
                    SchemaName = Defaults.SchemaName,
                    ConnectionName = Defaults.ConnectionName,
                    EventTableName = $"[TypeCode]_{Defaults.EventTableName}",
                }, action);

        public PostgreSQLEvenStoreConfigurator UseLocator(
            IRelationalEventStoreLocator relationalEventStoreLocator,
            Action<PostgreSQLEventStoreOptions>? action = null)
        {
            ConfigureOptions(providerOptions =>
            {
                var eventOptions = new PostgreSQLEventStoreOptions
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