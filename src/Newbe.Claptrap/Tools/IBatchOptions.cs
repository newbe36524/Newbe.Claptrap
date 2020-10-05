namespace Newbe.Claptrap
{
    public interface IBatchOptions
    {
        int? InsertManyWindowTimeInMilliseconds { get; }
        int? InsertManyWindowCount { get; }
        int? InsertManyMaxWindowCount { get; }
        int? InsertManyMinWindowCount { get; }
        bool? EnableAutoScale { get; }
    }
}