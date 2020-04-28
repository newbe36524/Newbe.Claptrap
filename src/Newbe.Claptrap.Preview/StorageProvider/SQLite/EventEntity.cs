using System;

namespace Newbe.Claptrap.Preview.SQLite
{
    public class EventEntity
    {
        public long Version { get; set; }
        public string Uid { get; set; }
        public string EventTypeCode { get; set; }
        public string EventData { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}