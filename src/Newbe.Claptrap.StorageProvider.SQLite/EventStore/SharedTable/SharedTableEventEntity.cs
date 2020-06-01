using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Newbe.Claptrap.StorageProvider.SQLite.EventStore.SharedTable
{
    public class SharedTableEventEntity
    {
        public string claptrap_type_code { get; set; } = null!;
        public string claptrap_id { get; set; } = null!;
        public long version { get; set; }
        public string event_type_code { get; set; } = null!;
        public string event_data { get; set; } = null!;
        public DateTime created_time { get; set; }

        public static IEnumerable<string> ParameterNames()
        {
            yield return nameof(claptrap_type_code);
            yield return nameof(claptrap_id);
            yield return nameof(version);
            yield return nameof(event_type_code);
            yield return nameof(event_data);
            yield return nameof(created_time);
        }

        public static IEnumerable<(string, Func<SharedTableEventEntity, object>)> ValueFactories()
        {
            yield return (nameof(claptrap_type_code), x => x.claptrap_type_code);
            yield return (nameof(claptrap_id), x => x.claptrap_id);
            yield return (nameof(version), x => x.version);
            yield return (nameof(event_type_code), x => x.event_type_code);
            yield return (nameof(event_data), x => x.event_data);
            yield return (nameof(created_time), x => x.created_time);
        }
    }
}