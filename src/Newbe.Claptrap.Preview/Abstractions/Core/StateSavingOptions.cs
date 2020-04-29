using System;

namespace Newbe.Claptrap.Preview.Abstractions.Core
{
    public class StateSavingOptions
    {
        public TimeSpan? SavingWindowTime { get; set; }
        public int? SavingWindowVersionLimit { get; set; }
        public bool SaveWhenDeactivateAsync { get; set; }
    }
}