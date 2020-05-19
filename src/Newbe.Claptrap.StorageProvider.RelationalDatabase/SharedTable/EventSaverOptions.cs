namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.SharedTable
{
    public class EventSaverOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; }
        public int? InsertManyWindowCount { get; set; }
        public bool AutoMigrationDb { get; set; }
    }
}