namespace Newbe.Claptrap
{
    public interface IClaptrapBox
    {
        IClaptrap Claptrap { get; }
    }

    public interface IClaptrapBox<out TStateData> : IClaptrapBox
        where TStateData : IStateData
    {
        TStateData StateData { get; }
    }
}