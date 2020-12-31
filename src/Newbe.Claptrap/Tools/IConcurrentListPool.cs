namespace Newbe.Claptrap
{
    public interface IConcurrentListPool<T>
    {
        ConcurrentList<T> Get(int size);
        void Return(ConcurrentList<T> list);
    }
}