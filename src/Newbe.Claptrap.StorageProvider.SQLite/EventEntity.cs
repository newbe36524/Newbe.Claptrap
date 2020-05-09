using System;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class EventEntity
    {
        public long Version { get; set; }
        public string Uid { get; set; } = null!;
        public string EventTypeCode { get; set; } = null!;
        public string EventData { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
    }
}