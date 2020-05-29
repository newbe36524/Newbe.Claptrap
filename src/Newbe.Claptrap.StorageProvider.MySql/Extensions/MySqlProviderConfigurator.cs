using System;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.MySql.Extensions
{
    public class MySqlProviderConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public MySqlProviderConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public MySqlProviderConfigurator AsEventStore(Action<MySqlEvenStoreConfigurator> eventStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.EventLoaderFactoryType = typeof(RelationalStoreFactory);
                        x.EventSaverFactoryType = typeof(RelationalStoreFactory);
                        var configurator = new MySqlEvenStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        eventStore(configurator);
                    });
            return this;
        }

        public MySqlProviderConfigurator AsStateStore(Action<MySqlStateStoreConfigurator> stateStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.StateLoaderFactoryType = typeof(RelationalStoreFactory);
                        x.StateSaverFactoryType = typeof(RelationalStoreFactory);
                        var configurator = new MySqlStateStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        stateStore(configurator);
                    });
            return this;
        }
    }
}