using Newbe.Claptrap.Preview.Abstractions.Box;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Orleans
{
    public interface IClaptrapBoxGrain<out TStateData> : IClaptrapBox<TStateData> where TStateData : IStateData
    {
        public IClaptrapGrainCommonService ClaptrapGrainCommonService { get; }
    }
}