using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Demo.Interfaces
{
    public class GrainEvent : IEvent
    {
        public IActorIdentity ActorIdentity { get; }
        public ulong Version { get; set; }
        public IEventUid Uid { get; }
        public string EventType { get; }
        public IEventData Data { get; }
    }
}