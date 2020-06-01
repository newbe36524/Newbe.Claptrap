using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.OneIdOneFile
{
    public class OneIdOneFileEventEntity
    {
        public long version { get; set; }
        public string event_type_code { get; set; } = null!;
        public string event_data { get; set; } = null!;
        public DateTime created_time { get; set; }
    }
}