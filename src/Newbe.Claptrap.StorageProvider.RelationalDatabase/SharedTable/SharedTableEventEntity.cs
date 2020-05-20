using System;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public class SharedTableEventEntity
    {
        public string ClaptrapTypeCode { get; set; }
        public string ClaptrapId { get; set; }
        public long Version { get; set; }
        public string EventTypeCode { get; set; }
        public string EventData { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}