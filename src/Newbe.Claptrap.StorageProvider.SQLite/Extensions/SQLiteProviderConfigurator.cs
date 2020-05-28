using System;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.Bootstrapper
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

        public SQLiteProviderConfigurator AsEventStore(Action<ISQLiteProviderEvenStoreConfigurator> eventStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.EventLoaderFactoryType = typeof(SQLiteStoreFactory);
                        x.EventSaverFactoryType = typeof(SQLiteStoreFactory);
                        var configurator = new SQLiteProviderEvenStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        eventStore(configurator);
                    });
            return this;
        }

        public SQLiteProviderConfigurator AsStateStore(Action<ISQLiteProviderEvenStoreConfigurator> stateStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.StateLoaderFactoryType = typeof(SQLiteStoreFactory);
                        x.StateSaverFactoryType = typeof(SQLiteStoreFactory);
                        var configurator = new SQLiteProviderEvenStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        stateStore(configurator);
                    });
            return this;
        }
    }
}