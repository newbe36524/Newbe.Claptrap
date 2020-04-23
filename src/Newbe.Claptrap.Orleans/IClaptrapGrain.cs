using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapGrain
    {
        IActor Actor { get; }
    }
    
    public interface IClaptrapGrain<out TStateData> : IClaptrapGrain
        where TStateData : IStateData
    {
        TStateData StateData { get; }
    }
}