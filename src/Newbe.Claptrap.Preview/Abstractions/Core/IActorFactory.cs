namespace Newbe.Claptrap.Preview.Core
{
    public interface IActorFactory
    {
        IActor Create(IActorIdentity identity);
    }
}