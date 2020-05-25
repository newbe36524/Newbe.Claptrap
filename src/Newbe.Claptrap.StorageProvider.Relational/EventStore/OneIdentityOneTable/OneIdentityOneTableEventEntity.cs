using System;

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore.OneIdentityOneTable
{
    public class OneIdentityOneTableEventEntity : IEventEntity
    {
        public long Version { get; set; }
        public string EventTypeCode { get; set; } = null!;
        public string EventData { get; set; } = null!;
        public DateTime CreatedTime { get; set; }
    }
}