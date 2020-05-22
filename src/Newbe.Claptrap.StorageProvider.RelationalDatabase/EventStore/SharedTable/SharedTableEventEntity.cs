using System;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable
{
    public class SharedTableEventEntity : IEventEntity
    {
        public string ClaptrapTypeCode { get; set; }
        public string ClaptrapId { get; set; }
        public long Version { get; set; }
        public string EventTypeCode { get; set; }
        public string EventData { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}