using System;

namespace Newbe.Claptrap.Preview
{
    public interface IClock
    {
        DateTime UtcNow { get; }
    }
}