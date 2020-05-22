namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.EventStore
{
    public class EventSaverOptions
    {
        public int? InsertManyWindowTimeInMilliseconds { get; set; }
        public int? InsertManyWindowCount { get; set; }
    }
}