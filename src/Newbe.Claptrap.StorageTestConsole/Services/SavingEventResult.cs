using System.Collections.Generic;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public record SavingEventResult
    {
        public int TotalCount { get; set; }
        public int BatchSize { get; set; }
        public int BatchCount { get; set; }
        [ReportIgnore] public List<long> BatchTimes { get; set; }
        public long TotalTime { get; set; }
        public double CountPerSecond => TotalCount * 1000.0 / TotalTime;
    }
}