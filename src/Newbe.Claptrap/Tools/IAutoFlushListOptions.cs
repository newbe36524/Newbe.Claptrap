using System;

namespace Newbe.Claptrap
{
    public interface IAutoFlushListOptions
    {
        int GetSize();
        TimeSpan GetDebounceTime();
        void SetLastFlushCount(int lastPushCount);
    }
}