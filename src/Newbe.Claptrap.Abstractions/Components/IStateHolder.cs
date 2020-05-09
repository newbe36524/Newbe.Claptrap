namespace Newbe.Claptrap
{
    public interface IStateHolder : IClaptrapComponent
    {
        IState DeepCopy(IState source);
    }
}