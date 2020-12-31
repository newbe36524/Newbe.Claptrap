using System;

// ReSharper disable InconsistentNaming

namespace Newbe.Claptrap.StorageProvider.Relational.EventStore
{
    public record RelationalEventEntity
    {
        public string claptrap_type_code { get; set; } = null!;
        public string claptrap_id { get; set; } = null!;
        public long version { get; set; }
        public string event_type_code { get; set; } = null!;
        public string event_data { get; set; } = null!;
        public DateTime created_time { get; set; }
    }
}