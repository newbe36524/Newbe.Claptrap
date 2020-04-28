using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Preview.Core;
using Newbe.Claptrap.Preview.StateStore;

namespace Newbe.Claptrap.Preview.SQLite
{
    [ExcludeFromCodeCoverage]
    public class SQLiteStateStoreFactoryHandler : IStateStoreFactoryHandler
    {
        private readonly SQLiteStateStore.Factory _sqLiteStateStoreFactory;

        public SQLiteStateStoreFactoryHandler(
            SQLiteStateStore.Factory sqLiteStateStoreFactory)
        {
            _sqLiteStateStoreFactory = sqLiteStateStoreFactory;
        }

        public IStateStore Create(IActorIdentity identity)
        {
            return _sqLiteStateStoreFactory(identity);
        }
    }
}