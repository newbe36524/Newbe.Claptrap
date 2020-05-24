using System;
using System.Collections.Generic;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore.SharedTable
{
    public class SharedTableEventEntity : IEventEntity
    {
        public string ClaptrapTypeCode { get; set; } = null!;
        public string ClaptrapId { get; set; } = null!;
        public long Version { get; set; }
        public string EventTypeCode { get; set; } = null!;
        public string EventData { get; set; } = null!;
        public DateTime CreatedTime { get; set; }

        public static IEnumerable<string> ParameterNames()
        {
            yield return nameof(ClaptrapTypeCode);
            yield return nameof(ClaptrapId);
            yield return nameof(Version);
            yield return nameof(EventTypeCode);
            yield return nameof(EventData);
            yield return nameof(CreatedTime);
        }

        public static IEnumerable<(string, Func<SharedTableEventEntity, object>)> ValueFactories()
        {
            yield return (nameof(ClaptrapTypeCode), x => x.ClaptrapTypeCode);
            yield return (nameof(ClaptrapId), x => x.ClaptrapId);
            yield return (nameof(Version), x => x.Version);
            yield return (nameof(EventTypeCode), x => x.EventTypeCode);
            yield return (nameof(EventData), x => x.EventData);
            yield return (nameof(CreatedTime), x => x.CreatedTime);
        }
    }
}