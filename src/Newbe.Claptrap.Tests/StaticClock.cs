using System;
using Newbe.Claptrap.Preview;

namespace Newbe.Claptrap.Tests
{
    public class StaticClock : IClock
    {
        public StaticClock(DateTime utcNow)
        {
            UtcNow = utcNow;
        }

        public DateTime UtcNow { get; }
    }
}