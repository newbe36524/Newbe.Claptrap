using Newbe.Claptrap.Preview.Core;

namespace Newbe.Claptrap.Preview.Orleans
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