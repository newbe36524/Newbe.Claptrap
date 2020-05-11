
namespace Newbe.Claptrap.Orleans
{
    public interface IClaptrapBoxGrain<out TStateData> : IClaptrapBox<TStateData> where TStateData : IStateData
    {
        public IClaptrapGrainCommonService ClaptrapGrainCommonService { get; }
    }
}