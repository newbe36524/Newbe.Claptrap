using System.Diagnostics.CodeAnalysis;
using Newbe.Claptrap.Core;
using Newbe.Claptrap.StateStore;

namespace Newbe.Claptrap.StorageProvider.SQLite
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