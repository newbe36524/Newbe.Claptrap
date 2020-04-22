using System.Collections.Generic;
using Newbe.Claptrap.Core;

namespace Newbe.Claptrap
{
    public class DataEvent : IEvent
    {
        public DataEvent(IActorIdentity actorIdentity, string eventTypeCode, IEventData data, IEventUid uid)
        {
            ActorIdentity = actorIdentity;
            Uid = uid;
            EventTypeCode = eventTypeCode;
            Data = data;
        }

        public IActorIdentity ActorIdentity { get; }
        public ulong Version { get; set; }
        public IEventUid Uid { get; }
        public string EventTypeCode { get; }
        public IEventData Data { get; }
    }
}