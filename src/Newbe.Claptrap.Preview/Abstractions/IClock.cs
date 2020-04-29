using System;

namespace Newbe.Claptrap.Preview.Abstractions
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}