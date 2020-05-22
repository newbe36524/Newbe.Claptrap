using System;

namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.StateStore.SharedTable
{
    public class SharedTableStateEntity : IStateEntity
    {
        public string ClaptrapTypeCode { get; set; }
        public string ClaptrapId { get; set; }
        public long Version { get; set; }
        public string StateData { get; set; }
        public DateTime UpdatedTime { get; set; }
    }
}