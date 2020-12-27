namespace Newbe.Claptrap.Dapr
{
    public interface IClaptrapBoxActor<out TStateData> : IClaptrapBox<TStateData> where TStateData : IStateData
    {
        public IClaptrapActorCommonService ClaptrapActorCommonService { get; }
    }
}