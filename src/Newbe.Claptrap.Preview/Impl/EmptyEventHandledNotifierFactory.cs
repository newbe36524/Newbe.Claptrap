using Newbe.Claptrap.Preview.Abstractions.Components;
using Newbe.Claptrap.Preview.Abstractions.Core;

namespace Newbe.Claptrap.Preview.Impl
{
    public class EmptyEventHandledNotifierFactory : IClaptrapComponentFactory<IEventHandledNotifier>
    {
        private readonly EmptyEventHandledNotifier.Factory _factory;

        public EmptyEventHandledNotifierFactory(
            EmptyEventHandledNotifier.Factory factory)
        {
            _factory = factory;
        }

        public IEventHandledNotifier Create(IClaptrapIdentity claptrapIdentity)
        {
            var re = _factory.Invoke(claptrapIdentity);
            return re;
        }
    }
}