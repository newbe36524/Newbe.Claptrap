using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.EventStore;

namespace Newbe.Claptrap.Preview.SQLite
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