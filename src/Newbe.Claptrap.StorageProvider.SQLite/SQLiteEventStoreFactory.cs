namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class SQLiteEventStoreFactory :
        IClaptrapComponentFactory<IEventLoader>,
        IClaptrapComponentFactory<IEventSaver>
    {
        private readonly SQLiteEventStore.Factory _factory;

        public SQLiteEventStoreFactory(
            SQLiteEventStore.Factory factory)
        {
            _factory = factory;
        }

        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }

        IEventSaver IClaptrapComponentFactory<IEventSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}