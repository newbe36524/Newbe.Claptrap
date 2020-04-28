using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventStore
{
    public interface IEventDataSerializer<T>
    {
        T Serialize(string actorTypeCode, string eventTypeCode, IEventData eventData);
        IEventData Deserialize(string actorTypeCode, string eventTypeCode, T source);
    }
}