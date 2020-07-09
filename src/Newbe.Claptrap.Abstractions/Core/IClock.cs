using System;

namespace Newbe.Claptrap
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}