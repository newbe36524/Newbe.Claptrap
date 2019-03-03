namespace Newbe.Claptrap.Core
{
    public interface IActorFactory
    {
        IActor Create(IActorIdentity identity);
    }
}