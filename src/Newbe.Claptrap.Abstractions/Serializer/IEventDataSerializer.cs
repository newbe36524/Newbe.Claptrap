namespace Newbe.Claptrap
{
    public interface IEventDataSerializer<T>
    {
        T Serialize(IClaptrapIdentity identity, string eventTypeCode, IEventData eventData);
        IEventData Deserialize(IClaptrapIdentity identity, string eventTypeCode, T source);
    }
}