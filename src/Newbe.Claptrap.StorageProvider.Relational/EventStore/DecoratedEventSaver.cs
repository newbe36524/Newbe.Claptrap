using System.Threading.Tasks;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public abstract class DecoratedEventSaver : IEventSaver
    {
        public IEventSaver StateSaver { get; }
        public IClaptrapIdentity Identity => StateSaver.Identity;

        protected DecoratedEventSaver(
            IEventSaver stateSaver)
        {
            StateSaver = stateSaver;
        }

        public abstract Task SaveEventAsync(IEvent @event);
    }
}