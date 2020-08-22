namespace Newbe.Claptrap
{
    public interface IStateDataSerializer<T>
    {
        T Serialize(IClaptrapIdentity identity, IStateData stateData);
        IStateData Deserialize(IClaptrapIdentity identity, T source);
    }
}