namespace Newbe.Claptrap.StorageProvider.MySql
{
    public class MySqlStateStoreFactory : IClaptrapComponentFactory<IStateLoader>,
        IClaptrapComponentFactory<IStateSaver>
    {
        private readonly MySqlStateStore.Factory _factory;

        public MySqlStateStoreFactory(
            MySqlStateStore.Factory factory)
        {
            _factory = factory;
        }

        IStateLoader IClaptrapComponentFactory<IStateLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory.Invoke(claptrapIdentity);
        }

        IStateSaver IClaptrapComponentFactory<IStateSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory.Invoke(claptrapIdentity);
        }
    }
}