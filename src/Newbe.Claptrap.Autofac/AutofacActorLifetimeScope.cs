using Newbe.Claptrap.Core;

namespace Newbe.Claptrap.Autofac
{
    public class AutofacActorLifetimeScope : IActorLifetimeScope
    {
        public IActorIdentity Identity { get; set; }
    }
}