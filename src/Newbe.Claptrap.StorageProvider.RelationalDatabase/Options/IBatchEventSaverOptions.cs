namespace Newbe.Claptrap.StorageProvider.RelationalDatabase.Options
{
    public interface IBatchEventSaverOptions :
        IEventSaverOptions
    {
        int? InsertManyWindowTimeInMilliseconds { get; }
        int? InsertManyWindowCount { get; }
    }
}