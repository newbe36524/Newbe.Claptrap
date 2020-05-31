using System;
using Newbe.Claptrap.Bootstrapper;
using Newbe.Claptrap.StorageProvider.Relational;

namespace Newbe.Claptrap.StorageProvider.MongoDB.Extensions
{
    public class MongoDBProviderConfigurator
    {
        private readonly Func<IClaptrapDesign, bool> _designFilter;
        private readonly IClaptrapBootstrapperBuilder _builder;

        public MongoDBProviderConfigurator(
            Func<IClaptrapDesign, bool> designFilter,
            IClaptrapBootstrapperBuilder builder)
        {
            _designFilter = designFilter;
            _builder = builder;
        }

        public MongoDBProviderConfigurator AsEventStore(Action<MongoDBEvenStoreConfigurator> eventStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.EventLoaderFactoryType = typeof(RelationalStoreFactory);
                        x.EventSaverFactoryType = typeof(RelationalStoreFactory);
                        var configurator = new MongoDBEvenStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        eventStore(configurator);
                    });
            return this;
        }

        public MongoDBProviderConfigurator AsStateStore(Action<MongoDBStateStoreConfigurator> stateStore)
        {
            _builder
                .ConfigureClaptrapDesign(
                    _designFilter,
                    x =>
                    {
                        x.StateLoaderFactoryType = typeof(RelationalStoreFactory);
                        x.StateSaverFactoryType = typeof(RelationalStoreFactory);
                        var configurator = new MongoDBStateStoreConfigurator(x.ClaptrapStorageProviderOptions);
                        stateStore(configurator);
                    });
            return this;
        }
    }
}