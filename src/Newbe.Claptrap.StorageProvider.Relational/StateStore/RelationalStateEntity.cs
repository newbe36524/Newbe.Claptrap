using System;
using System.Collections.Generic;

// ReSharper disable InconsistentNaming

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore
{
    public class RelationalStateEntity
    {
        public string claptrap_type_code { get; set; } = null!;
        public string claptrap_id { get; set; } = null!;
        public long version { get; set; }
        public string state_data { get; set; } = null!;
        public DateTime updated_time { get; set; }

        public static IEnumerable<string> ParameterNames()
        {
            yield return nameof(claptrap_type_code);
            yield return nameof(claptrap_id);
            yield return nameof(version);
            yield return nameof(state_data);
            yield return nameof(updated_time);
        }

        public static IEnumerable<(string, Func<RelationalStateEntity, object>)> ValueFactories()
        {
            yield return (nameof(claptrap_type_code), x => x.claptrap_type_code);
            yield return (nameof(claptrap_id), x => x.claptrap_id);
            yield return (nameof(version), x => x.version);
            yield return (nameof(state_data), x => x.state_data);
            yield return (nameof(updated_time), x => x.updated_time);
        }
    }
}