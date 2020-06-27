namespace Newbe.Claptrap.CapacityBurning.Services
{
    public class EventSavingBurningOptions
    {
        public int UserIdCount { get; set; } = 10;
        public int BatchSize { get; set; } = 5000;
        public int BatchCount { get; set; } = 100;
        public int ConcurrentCount { get; set; } = 100;
        public int PreparingSleepInSec { get; set; } = 30;
    }
}