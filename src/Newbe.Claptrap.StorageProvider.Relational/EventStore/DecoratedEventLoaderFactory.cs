using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class DecoratedEventLoaderFactory<TImpl>
        : DecoratedClaptrapComponentFactory<TImpl, IEventLoader>
        where TImpl : IEventLoader
    {
        public DecoratedEventLoaderFactory(Func<IEventLoader, TImpl> func,
            RelationalStoreFactory relationalStoreFactory) : base(func,
            relationalStoreFactory)
        {
        }
    }
}