namespace Newbe.Claptrap
{
    public class NoChangeStateHolderFactory : IClaptrapComponentFactory<IStateHolder>
    {
        private readonly NoChangeStateHolder.Factory _factory;

        public NoChangeStateHolderFactory(
            NoChangeStateHolder.Factory factory)
        {
            _factory = factory;
        }

        public IStateHolder Create(IClaptrapIdentity claptrapIdentity)
        {
            return _factory(claptrapIdentity);
        }
    }
}