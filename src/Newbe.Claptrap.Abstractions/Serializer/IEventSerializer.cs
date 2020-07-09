namespace Newbe.Claptrap
{
    public interface IEventSerializer<T>
    {
        T Serialize(IEvent evt);
        IEvent Deserialize(T source);
    }
}