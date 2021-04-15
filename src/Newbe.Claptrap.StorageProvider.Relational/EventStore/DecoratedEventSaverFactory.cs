using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class DecoratedEventSaverFactory<TImpl>
        : DecoratedClaptrapComponentFactory<TImpl, IEventSaver>
        where TImpl : IEventSaver
    {
        public DecoratedEventSaverFactory(Func<IEventSaver, TImpl> func,
            RelationalStoreFactory relationalStoreFactory) : base(func,
            relationalStoreFactory)
        {
        }
    }
}