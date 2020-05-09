namespace Newbe.Claptrap.Serializer
{
    public interface IEventDataSerializer<T>
    {
        T Serialize(string claptrapTypeCode, string eventTypeCode, IEventData eventData);
        IEventData Deserialize(string claptrapTypeCode, string eventTypeCode, T source);
    }
}