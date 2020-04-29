using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Serializer
{
    public interface IEventDataSerializer<T>
    {
        T Serialize(string claptrapTypeCode, string eventTypeCode, IEventData eventData);
        IEventData Deserialize(string claptrapTypeCode, string eventTypeCode, T source);
    }
}