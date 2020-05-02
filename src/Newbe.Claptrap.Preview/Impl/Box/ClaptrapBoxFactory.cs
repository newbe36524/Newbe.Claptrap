using Newbe.Claptrap.Preview.Abstractions.Box;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl.Box
{
    public class ClaptrapBoxFactory : IClaptrapBoxFactory
    {
        private readonly NormalClaptrapBox.Factory _factory;

        public ClaptrapBoxFactory(
            NormalClaptrapBox.Factory factory)
        {
            _factory = factory;
        }

        public IClaptrapBox Create(IClaptrapIdentity identity)
        {
            return _factory.Invoke(identity);
        }
    }
}