using System;
using Newbe.Claptrap.Preview;
using Newbe.Claptrap.Preview.Abstractions;

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