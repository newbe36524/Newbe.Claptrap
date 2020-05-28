using Autofac;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class RelationalEventStoreFactory :
        IClaptrapComponentFactory<IEventSaver>,
        IClaptrapComponentFactory<IEventLoader>
    {
        private readonly ILifetimeScope _lifetimeScope;

        public RelationalEventStoreFactory(
            ILifetimeScope lifetimeScope)
        {
            _lifetimeScope = lifetimeScope;
        }

        IEventSaver IClaptrapComponentFactory<IEventSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _lifetimeScope.Resolve<IRelationalEventSaver>();
        }

        IEventLoader IClaptrapComponentFactory<IEventLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _lifetimeScope.Resolve<IRelationalEventLoader>();
        }
    }
}