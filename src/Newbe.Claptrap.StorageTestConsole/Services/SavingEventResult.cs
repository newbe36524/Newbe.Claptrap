using System.Collections.Generic;
using System.Linq;

namespace Newbe.Claptrap.StorageTestConsole.Services
{
    public class SavingEventResult
    {
        public int TotalCount { get; set; }
        public int BatchSize { get; set; }
        public int BatchCount { get; set; }
        [ReportIgnore] public List<long> BatchTimes { get; set; }
        public long TotalTime => BatchTimes.Sum();
        public double CountPerMs => TotalCount * 1000.0 / TotalTime;
    }
}