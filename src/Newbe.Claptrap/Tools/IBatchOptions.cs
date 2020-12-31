namespace Newbe.Claptrap
{
    public interface IBatchOptions
    {
        int? InsertManyWindowTimeInMilliseconds { get; }
        int? InsertManyWindowCount { get; }
        int? WorkerCount { get; }
    }
}