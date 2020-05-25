using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore.OneTypeOneTable
{
    public class OneTypeOneTableEventEntity : IEventEntity
    {
        public string ClaptrapId { get; set; }
        public long Version { get; set; }
        public string EventTypeCode { get; set; } = null!;
        public string EventData { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
    }
}