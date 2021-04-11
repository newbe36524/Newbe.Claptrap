using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class DecoratedEventLoaderFactory<TImpl>
        : DecoratedClaptrapComponentFactory<TImpl, IEventLoader>
        where TImpl : IEventLoader
    {
        public DecoratedEventLoaderFactory(Func<IStateLoader, TImpl> func,
            RelationalStoreFactory relationalStoreFactory) : base(func,
            relationalStoreFactory)
        {
        }
    }
}