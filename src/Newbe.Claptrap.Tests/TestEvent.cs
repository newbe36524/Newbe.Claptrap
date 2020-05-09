namespace Newbe.Claptrap.Tests
{
    public class TestEvent : IEvent
    {
        public IClaptrapIdentity ClaptrapIdentity { get; set; }
        public long Version { get; set; }
        public string Uid { get; set; }
        public string EventTypeCode { get; set; }
        public IEventData Data { get; set; }
    }
}