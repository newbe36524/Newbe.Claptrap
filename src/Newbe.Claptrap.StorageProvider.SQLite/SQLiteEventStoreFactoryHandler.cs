using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.EventStore;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    [ExcludeFromCodeCoverage]
    public class SQLiteEventStoreFactoryHandler : IIEventStoreFactoryHandler
    {
        private readonly SQLiteEventStore.Factory _sqLiteEventStoreFactory;

        public SQLiteEventStoreFactoryHandler(
            SQLiteEventStore.Factory sqLiteEventStoreFactory)
        {
            _sqLiteEventStoreFactory = sqLiteEventStoreFactory;
        }

        public IEventStore Create(IActorIdentity identity)
        {
            return _sqLiteEventStoreFactory(identity);
        }
    }
}