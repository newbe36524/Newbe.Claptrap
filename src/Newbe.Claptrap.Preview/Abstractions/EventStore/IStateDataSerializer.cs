using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.EventStore
{
    public interface IStateDataSerializer<T>
    {
        T Serialize(string actorTypeCode, IStateData stateData);
        IStateData Deserialize(string actorTypeCode, T source);
    }
}