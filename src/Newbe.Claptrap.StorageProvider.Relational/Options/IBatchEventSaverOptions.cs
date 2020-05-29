namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IBatchEventSaverOptions :
        IEventSaverOptions
    {
        int? InsertManyWindowTimeInMilliseconds { get; }
        int? InsertManyWindowCount { get; }
    }
}