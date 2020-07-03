namespace Newbe.Claptrap
{
    public class EmptyEventSaverFactory : IClaptrapComponentFactory<IEventSaver>
    {
        public IEventSaver Create(IClaptrapIdentity claptrapIdentity)
        {
            return null!;
        }
    }
}