using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Newbe.Claptrap.Box;
using Newbe.Claptrap.StorageProvider.Relational.EventStore;
using Newbe.Claptrap.StorageProvider.Relational.StateStore;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    [ClaptrapStateStore(
        typeof(DecoratedStateSaverFactory<MyStateSaver>),
        typeof(DecoratedStateLoaderFactory<MyStateLoader>))]
    [ClaptrapEventStore(
        typeof(DecoratedEventSaverFactory<MyEventSaver>),
        typeof(DecoratedEventLoaderFactory<MyEventLoader>))]
    public class CustomFactoryClaptrap : NormalClaptrapBox<AccountState>, ICustomFactoryClaptrap
    {
        public CustomFactoryClaptrap(IClaptrapIdentity identity,
            IClaptrapFactory claptrapFactory,
            IClaptrapAccessor claptrapAccessor) : base(identity,
            claptrapFactory,
            claptrapAccessor)
        {
        }
    }

    [ClaptrapState(typeof(CustomFactoryClaptrapState), Codes.CustomFactoryClaptrap)]
    public interface ICustomFactoryClaptrap
    {
    }

    public record CustomFactoryClaptrapState : IStateData
    {
    }

    public class MyStateLoader : DecoratedStateLoader
    {
        public static bool Touched { get; private set; }

        public MyStateLoader(IStateLoader stateLoader) : base(stateLoader)
        {
        }

        public override Task<IState?> GetStateSnapshotAsync()
        {
            Touched = true;
            return Task.FromResult((IState) new UnitState());
        }
    }

    public class MyStateSaver : DecoratedStateSaver
    {
        public static bool Touched { get; private set; }

        public MyStateSaver(IStateSaver stateSaver) : base(stateSaver)
        {
        }

        public override Task SaveAsync(IState state)
        {
            Touched = true;
            return Task.CompletedTask;
        }
    }

    public class MyEventLoader : DecoratedEventLoader
    {
        public static bool Touched { get; private set; }

        public MyEventLoader(IEventLoader loader) : base(loader)
        {
        }

        public override Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            Touched = true;
            return Task.FromResult(Enumerable.Empty<IEvent>());
        }
    }

    public class MyEventSaver : DecoratedEventSaver
    {
        public static bool Touched { get; private set; }

        public MyEventSaver(IEventSaver stateSaver) : base(stateSaver)
        {
        }

        public override Task SaveEventAsync(IEvent @event)
        {
            Touched = true;
            return Task.CompletedTask;
        }
    }

    public class CustomLoaderAndSaverModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<MyStateLoader>().AsSelf();
            builder.RegisterType<MyStateSaver>().AsSelf();
            builder.RegisterType<MyEventLoader>().AsSelf();
            builder.RegisterType<MyEventSaver>().AsSelf();
        }
    }
}