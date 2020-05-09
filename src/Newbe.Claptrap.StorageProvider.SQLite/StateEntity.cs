using System;

namespace Newbe.Claptrap.StorageProvider.SQLite
{
    public class StateEntity
    {
        public string Id { get; set; } = null!;
        public long Version { get; set; }
        public string ActorTypeCode { get; set; } = null!;
        public string StateData { get; set; } = null!;
        public DateTime UpdatedTime { get; set; }
    }
}