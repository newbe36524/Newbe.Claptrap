namespace Newbe.Claptrap.CapacityBurning.Services
{
    public class StateSavingBurningOptions
    {
        public int UserIdCount { get; set; } = 10;
        public int VersionRange { get; set; } = 5000;
        public int RepeatCount { get; set; } = 100;
        public int ConcurrentCount { get; set; } = 100;
        public int PreparingSleepInSec { get; set; } = 30;
    }
}