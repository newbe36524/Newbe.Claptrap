using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview
{
    public class DataEvent : IEvent
    {
        public DataEvent(IActorIdentity actorIdentity, string eventTypeCode, IEventData data, string? uid)
        {
            ActorIdentity = actorIdentity;
            Uid = uid;
            EventTypeCode = eventTypeCode;
            Data = data;
        }

        public IActorIdentity ActorIdentity { get; }
        public long Version { get; set; }
        public string? Uid { get; }
        public string EventTypeCode { get; }
        public IEventData Data { get; }
    }
}