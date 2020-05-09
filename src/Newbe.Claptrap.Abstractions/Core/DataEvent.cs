namespace Newbe.Claptrap
{
    public class DataEvent : IEvent
    {
        public DataEvent(IClaptrapIdentity claptrapIdentity, string eventTypeCode, IEventData data, string? uid)
        {
            ClaptrapIdentity = claptrapIdentity;
            Uid = uid;
            EventTypeCode = eventTypeCode;
            Data = data;
        }

        public IClaptrapIdentity ClaptrapIdentity { get; }
        public long Version { get; set; }
        public string? Uid { get; }
        public string EventTypeCode { get; }
        public IEventData Data { get; }
    }
}