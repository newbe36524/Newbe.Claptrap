using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public class EventEntity : IEventEntity
    {
        public string ClaptrapTypeCode { get; set; } = null!;
        public string ClaptrapId { get; set; } = null!;
        public long Version { get; set; }
        public string EventTypeCode { get; set; } = null!;
        public string EventData { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
    }
}