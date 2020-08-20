using System.Threading.Tasks;
using Autofac;

namespace Newbe.Claptrap.Saga
{
    public class DisposableSagaClaptrap : IDisposableSagaClaptrap
    {
        private readonly ISagaClaptrap _sagaClaptrap;
        private readonly ILifetimeScope _lifetimeScope;

        public DisposableSagaClaptrap(
            ISagaClaptrap sagaClaptrap,
            ILifetimeScope lifetimeScope)
        {
            _sagaClaptrap = sagaClaptrap;
            _lifetimeScope = lifetimeScope;
        }

        public Task RunAsync(SagaFlow flow)
        {
            return _sagaClaptrap.RunAsync(flow);
        }

        public Task ContinueAsync()
        {
            return _sagaClaptrap.ContinueAsync();
        }

        public ValueTask DisposeAsync()
        {
            return _lifetimeScope.DisposeAsync();
        }
    }
}