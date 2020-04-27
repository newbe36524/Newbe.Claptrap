using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.EventStore
{
    public interface IStateDataSerializer<T>
    {
        T Serialize(string actorTypeCode, IStateData stateData);
        IStateData Deserialize(string actorTypeCode, T source);
    }
}