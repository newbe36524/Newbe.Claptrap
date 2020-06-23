namespace Newbe.Claptrap.StorageProvider.Relational.Options
{
    public interface IBatchSaverOptions
    {
        int? InsertManyWindowTimeInMilliseconds { get; }
        int? InsertManyWindowCount { get; }
    }
}