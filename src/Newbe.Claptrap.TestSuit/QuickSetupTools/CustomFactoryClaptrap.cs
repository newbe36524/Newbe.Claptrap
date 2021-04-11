using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newbe.Claptrap.Box;

namespace Newbe.Claptrap.TestSuit.QuickSetupTools
{
    [ClaptrapStateStore(typeof(StateSaverFactory), typeof(StateLoaderFactory))]
    [ClaptrapEventStore(typeof(EventSaverFactory), typeof(EventLoaderFactory))]
    public class CustomFactoryClaptrap : NormalClaptrapBox<AccountState>, ICustomFactoryClaptrap
    {
        public delegate CustomFactoryClaptrap Factory(IClaptrapIdentity identity);

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

    public class StateLoaderFactory : IClaptrapComponentFactory<IStateLoader>
    {
        public IStateLoader Create(IClaptrapIdentity claptrapIdentity)
        {
            return new MyStateLoader(claptrapIdentity);
        }
    }

    public class MyStateLoader : IStateLoader
    {
        public static bool Touched { get; private set; }
        public MyStateLoader(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task<IState?> GetStateSnapshotAsync()
        {
            Touched = true;
            return Task.FromResult((IState) new UnitState());
        }
    }

    public class StateSaverFactory : IClaptrapComponentFactory<IStateSaver>
    {
        public IStateSaver Create(IClaptrapIdentity claptrapIdentity)
        {
            return new MyStateSaver(claptrapIdentity);
        }
    }

    public class MyStateSaver : IStateSaver
    {
        public static bool Touched { get; private set; }
        public MyStateSaver(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task SaveAsync(IState state)
        {
            Touched = true;
            return Task.CompletedTask;
        }
    }

    public class EventLoaderFactory : IClaptrapComponentFactory<IEventLoader>
    {
        public IEventLoader Create(IClaptrapIdentity claptrapIdentity)
        {
            return new MyEventLoader(claptrapIdentity);
        }
    }

    public class MyEventLoader : IEventLoader
    {
        public static bool Touched { get; private set; }
        public MyEventLoader(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task<IEnumerable<IEvent>> GetEventsAsync(long startVersion, long endVersion)
        {
            Touched = true;
            return Task.FromResult(Enumerable.Empty<IEvent>());
        }
    }

    public class EventSaverFactory : IClaptrapComponentFactory<IEventSaver>
    {
        public IEventSaver Create(IClaptrapIdentity claptrapIdentity)
        {
            return new MyEventSaver(claptrapIdentity);
        }
    }

    public class MyEventSaver : IEventSaver
    {
        public static bool Touched { get; private set; }
        public MyEventSaver(IClaptrapIdentity identity)
        {
            Identity = identity;
        }

        public IClaptrapIdentity Identity { get; }

        public Task SaveEventAsync(IEvent @event)
        {
            Touched = true;
            return Task.CompletedTask;
        }
    }
}