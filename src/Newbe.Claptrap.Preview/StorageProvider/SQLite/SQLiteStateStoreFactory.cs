using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.StorageProvider.SQLite
{
    public class SQLiteStateStoreFactory :
        IClaptrapComponentFactory<IStateSaver>,
        IClaptrapComponentFactory<IStateLoader>
    {
        private readonly SQLiteStateStore.Factory _factory;

        public SQLiteStateStoreFactory(
            SQLiteStateStore.Factory factory)
        {
            _factory = factory;
        }

        IStateSaver IClaptrapComponentFactory<IStateSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }

        IStateLoader IClaptrapComponentFactory<IStateLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}