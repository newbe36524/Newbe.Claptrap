using System;

namespace Newbe.Claptrap.StorageProvider.Relational.StateStore.SharedTable
{
    public class SharedTableStateEntity : IStateEntity
    {
        public string ClaptrapTypeCode { get; set; } = null!;
        public string ClaptrapId { get; set; } = null!;
        public long Version { get; set; }
        public string StateData { get; set; } = null!;
        public DateTime UpdatedTime { get; set; }
    }
}