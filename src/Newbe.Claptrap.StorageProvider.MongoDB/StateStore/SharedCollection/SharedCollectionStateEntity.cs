using System;
using MongoDB.Bson.Serialization.Attributes;

// ReSharper disable InconsistentNaming

namespace Newbe.Claptrap.StorageProvider.MongoDB.StateStore.SharedCollection
{
    [BsonIgnoreExtraElements]
    public class SharedCollectionStateEntity
    {
        public string claptrap_type_code { get; set; } = null!;
        public string claptrap_id { get; set; } = null!;
        public long version { get; set; }
        public string state_data { get; set; } = null!;
        public DateTime updated_time { get; set; }
    }
}