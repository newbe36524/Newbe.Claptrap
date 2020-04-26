using System;

namespace Newbe.Claptrap.Tests
{
    public class FuncClock : IClock
    {
        private readonly Func<DateTime> _func;

        public FuncClock(
            Func<DateTime> func)
        {
            _func = func;
        }

        public DateTime UtcNow => _func();
    }
}