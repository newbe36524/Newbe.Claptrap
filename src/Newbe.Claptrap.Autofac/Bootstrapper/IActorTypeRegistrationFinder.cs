using System.Collections.Generic;
using System.Reflection;

namespace Newbe.Claptrap.Autofac
{
    public interface IActorTypeRegistrationFinder
    {
        IEnumerable<ActorTypeRegistration> FindActors(IEnumerable<Assembly> assemblies);

        IEnumerable<EventHandlerTypeRegistration> FindEventHandlers(IEnumerable<Assembly> assemblies,
            IEnumerable<ActorTypeRegistration> actorTypeRegistrations);
    }
}