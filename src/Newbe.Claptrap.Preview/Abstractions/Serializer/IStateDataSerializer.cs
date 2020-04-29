using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Abstractions.Serializer
{
    public interface IStateDataSerializer<T>
    {
        T Serialize(string claptrapTypeCode, IStateData stateData);
        IStateData Deserialize(string claptrapTypeCode, T source);
    }
}