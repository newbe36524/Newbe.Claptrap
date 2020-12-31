using System;

namespace Newbe.Claptrap
{
    public class StaticAutoFlushListOptions : IAutoFlushListOptions
    {
        private readonly int _size;
        private readonly TimeSpan _debounceTime;

        public StaticAutoFlushListOptions(
            int size,
            TimeSpan debounceTime)
        {
            _size = size;
            _debounceTime = debounceTime;
        }

        public int GetSize()
        {
            return _size;
        }

        public TimeSpan GetDebounceTime()
        {
            return _debounceTime;
        }

        public void SetLastFlushCount(int lastPushCount)
        {
            // do nothing
        }
    }
}