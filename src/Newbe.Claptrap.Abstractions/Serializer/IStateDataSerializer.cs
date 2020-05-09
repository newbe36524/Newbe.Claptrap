namespace Newbe.Claptrap.Serializer
{
    public interface IStateDataSerializer<T>
    {
        T Serialize(string claptrapTypeCode, IStateData stateData);
        IStateData Deserialize(string claptrapTypeCode, T source);
    }
}