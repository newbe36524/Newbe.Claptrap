namespace Newbe.Claptrap
{
    public interface IClaptrapComponentFactory<out T>
        where T : class, IClaptrapComponent
    {
        T Create(IClaptrapIdentity claptrapIdentity);
    }
}