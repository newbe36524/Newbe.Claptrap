using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Tests
{
    public class TestEvent : IEvent
    {
        public IActorIdentity ActorIdentity { get; set; }
        public long Version { get; set; }
        public string Uid { get; set; }
        public string EventTypeCode { get; set; }
        public IEventData Data { get; set; }
    }
}