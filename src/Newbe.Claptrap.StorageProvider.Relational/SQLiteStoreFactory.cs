using Autofac;

namespace Newbe.Claptrap.StorageProvider.Relational
{
    public class SQLiteStoreFactory :
        IClaptrapComponentFactory<IEventSaver>,
        IClaptrapComponentFactory<IEventLoader>,
        IClaptrapComponentFactory<IStateSaver>,
        IClaptrapComponentFactory<IStateLoader>
    {
        private readonly ILifetimeScope _lifetimeScope;

        public SQLiteStoreFactory(
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

        IStateSaver IClaptrapComponentFactory<IStateSaver>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _lifetimeScope.Resolve<IRelationalStateSaver>();
        }

        IStateLoader IClaptrapComponentFactory<IStateLoader>.Create(IClaptrapIdentity claptrapIdentity)
        {
            return _lifetimeScope.Resolve<IRelationalStateLoader>();
        }
    }
}