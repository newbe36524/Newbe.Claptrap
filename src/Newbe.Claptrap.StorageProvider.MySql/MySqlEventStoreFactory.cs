namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlEventStoreFactory : IClaptrapComponentFactory<IEventLoader>,
        IClaptrapComponentFactory<IEventSaver>
    {
        private readonly MySqlEventStore.Factory _factory;

        public MySqlEventStoreFactory(
            MySqlEventStore.Factory factory)
        {
            _factory = factory;
        }

        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory.Invoke(claptrapIdentity);
        }

        IEventSaver IClaptrapComponentFactory<IEventSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory.Invoke(claptrapIdentity);
        }
    }
}