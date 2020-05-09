using System;
using System.Diagnostics.CodeAnalysis;

namespace Newbe.Claptrap
{
    [ExcludeFromCodeCoverage]
    public class SystemClock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}