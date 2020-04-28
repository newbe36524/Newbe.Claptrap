using System;
using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap.Preview
{
    [ExcludeFromCodeCoverage]
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}