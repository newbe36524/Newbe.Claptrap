using System;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class DecoratedStateLoaderFactory<TImpl>
        : DecoratedClaptrapComponentFactory<TImpl, IStateLoader>
        where TImpl : IStateLoader
    {
        public DecoratedStateLoaderFactory(Func<IStateLoader, TImpl> func,
            RelationalStoreFactory relationalStoreFactory) : base(func,
            relationalStoreFactory)
        {
        }
    }
}