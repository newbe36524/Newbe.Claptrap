using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrap
    {
        IActor Actor { get; }
    }
    
    public interface IClaptrap<out TStateData> : IClaptrap
        where TStateData : IStateData
    {
        TStateData StateData { get; }
    }
}