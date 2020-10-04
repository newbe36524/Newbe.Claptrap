using System;

namespace Newbe.Claptrap
{
    public struct ExceptionConcurrentList<T>
    {
        public ConcurrentList<T> ConcurrentList { get; set; }
        public Exception? Exception { get; set; }

        public void Deconstruct(out ConcurrentList<T> concurrentList, out Exception? exception)
        {
            concurrentList = ConcurrentList;
            exception = Exception;
        }
    }
}