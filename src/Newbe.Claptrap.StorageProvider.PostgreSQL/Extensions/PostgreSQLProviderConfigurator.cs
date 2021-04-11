using System;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.PostgreSQL.Extensions
{
    public class PostgreSQLProviderConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public PostgreSQLProviderConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public PostgreSQLProviderConfigurator AsEventStore(Action<PostgreSQLEvenStoreConfigurator> eventStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.EventLoaderFactoryType ??= typeof(RelationalStoreFactory);
                        x.EventSaverFactoryType ??= typeof(RelationalStoreFactory);
                        var configurator = new PostgreSQLEvenStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        eventStore(configurator);
                    });
            return this;
        }

        public PostgreSQLProviderConfigurator AsStateStore(Action<PostgreSQLStateStoreConfigurator> stateStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.StateLoaderFactoryType ??= typeof(RelationalStoreFactory);
                        x.StateSaverFactoryType ??= typeof(RelationalStoreFactory);
                        var configurator = new PostgreSQLStateStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        stateStore(configurator);
                    });
            return this;
        }
    }
}