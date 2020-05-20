using System;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.OneTypeOneTable
{
    public class OneTypeOneTableEventEntity
    {
        public long Version { get; set; }
        public string EventTypeCode { get; set; } = null!;
        public string EventData { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
    }
}