using System;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.SQLite.Extensions
{
    public class SQLiteProviderConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public SQLiteProviderConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public SQLiteProviderConfigurator AsEventStore(Action<SQLiteEvenStoreConfigurator> eventStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.EventLoaderFactoryType = typeof(RelationalStoreFactory);
                        x.EventSaverFactoryType = typeof(RelationalStoreFactory);
                        var configurator = new SQLiteEvenStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        eventStore(configurator);
                    });
            return this;
        }

        public SQLiteProviderConfigurator AsStateStore(Action<SQLiteStateStoreConfigurator> stateStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.StateLoaderFactoryType = typeof(RelationalStoreFactory);
                        x.StateSaverFactoryType = typeof(RelationalStoreFactory);
                        var configurator = new SQLiteStateStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        stateStore(configurator);
                    });
            return this;
        }
    }
}