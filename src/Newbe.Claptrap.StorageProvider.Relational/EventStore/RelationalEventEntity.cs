using System;
using System.Collections.Generic;
using System.Data;

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

        public static IEnumerable<string> ParameterNames()
        {
            yield return nameof(claptrap_type_code);
            yield return nameof(claptrap_id);
            yield return nameof(version);
            yield return nameof(event_type_code);
            yield return nameof(event_data);
            yield return nameof(created_time);
        }

        private static readonly (string, Func<RelationalEventEntity, object>)[] Values =
        {
            (nameof(claptrap_type_code), x => x.claptrap_type_code),
            (nameof(claptrap_id), x => x.claptrap_id),
            (nameof(version), x => x.version),
            (nameof(event_type_code), x => x.event_type_code),
            (nameof(event_data), x => x.event_data),
            (nameof(created_time), x => x.created_time),
        };

        public static IEnumerable<(string, Func<RelationalEventEntity, object>)> ValueFactories => Values;

        public static void FillParameter(RelationalEventEntity entity,
            IDbCommand cmd,
            IAdoParameterCache cache,
            int index)
        {
            cmd.Parameters.Clear();
            // gc free
            IDataParameter dataParameter;
            dataParameter = cache.GetParameter(nameof(claptrap_type_code), index);
            dataParameter.Value = entity.claptrap_type_code;
            cmd.Parameters.Add(dataParameter);
            dataParameter = cache.GetParameter(nameof(claptrap_id), index);
            dataParameter.Value = entity.claptrap_id;
            cmd.Parameters.Add(dataParameter);
            dataParameter = cache.GetParameter(nameof(version), index);
            dataParameter.Value = entity.version;
            cmd.Parameters.Add(dataParameter);
            dataParameter = cache.GetParameter(nameof(event_type_code), index);
            dataParameter.Value = entity.event_type_code;
            cmd.Parameters.Add(dataParameter);
            dataParameter = cache.GetParameter(nameof(event_data), index);
            dataParameter.Value = entity.event_data;
            cmd.Parameters.Add(dataParameter);
            dataParameter = cache.GetParameter(nameof(created_time), index);
            dataParameter.Value = entity.created_time;
            cmd.Parameters.Add(dataParameter);
        }
    }
}