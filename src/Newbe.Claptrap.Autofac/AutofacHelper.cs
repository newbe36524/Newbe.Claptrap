using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Newbe.Claptrap.EventHandler;
using Newbe.Claptrap.StateInitializer;

namespace Newbe.Claptrap.Autofac
{
    public static class AutofacHelper
    {
        public static void PerActorScope<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
        {
            builder.InstancePerMatchingLifetimeScope(Constants.ActorLifetimeScope);
        }

        public static void PerEventScope<TLimit, TActivatorData, TRegistrationStyle>(
            this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> builder)
        {
            builder.InstancePerMatchingLifetimeScope(Constants.EventLifetimeScope);
        }

        public static void RegisterEventMethods(this ContainerBuilder builder, IEnumerable<Assembly> assemblies)
        {
            var eventMethodRegistrationFinder = new EventMethodRegistrationFinder();
            var allTypes = assemblies.SelectMany(x => x.GetTypes()).ToArray();
            var eventMethodRegistrations = eventMethodRegistrationFinder.FindAll(allTypes);
            foreach (var eventMethodRegistration in eventMethodRegistrations)
            {
                builder.RegisterType(eventMethodRegistration.Type)
                    .AsImplementedInterfaces();
            }
        }

        public static void RegisterUpdateStateDataHandlers(this ContainerBuilder builder,
            IEnumerable<Assembly> assemblies)
        {
            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            var provider = new ReflectionActorMetadataProvider(new[] {new ActorAssemblyProvider(assemblyArray)});
            IStateDataUpdaterRegistrationFinder finder = new StateDataUpdaterRegistrationFinder(provider);
            var allTypes = assemblyArray.SelectMany(x => x.GetTypes()).ToArray();
            var registrations = finder.FindAll(allTypes);
            foreach (var registration in registrations)
            {
                builder.RegisterType(registration.Type)
                    .Keyed<IStateDataUpdater>(registration.Key);
            }
        }

        public static void RegisterDefaultStateDataFactories(this ContainerBuilder builder,
            IEnumerable<Assembly> assemblies)
        {
            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            var provider = new ReflectionActorMetadataProvider(new[] {new ActorAssemblyProvider(assemblyArray)});
            var allTypes = assemblyArray.SelectMany(x => x.GetTypes()).ToArray();
            IStateDataFactoryFinder finder = new StateDataFactoryFinder(provider);
            var registrations = finder.FindAll(allTypes);
            foreach (var registration in registrations)
            {
                builder.RegisterType(registration.Type)
                    .Keyed<IStateDataFactory>(registration.Key);
            }
        }

        public static void RegisterMinionEventHandler(this ContainerBuilder builder,
            IEnumerable<Assembly> assemblies)
        {
            var assemblyArray = assemblies as Assembly[] ?? assemblies.ToArray();
            var allTypes = assemblyArray.SelectMany(x => x.GetTypes()).ToArray();
            var provider = new ReflectionActorMetadataProvider(new[] {new ActorAssemblyProvider(assemblyArray),});
            IMinionEventHandlerFinder finder = new MinionEventHandlerFinder(provider);
            var registrations = finder.FindAll(allTypes);

            foreach (var registration in registrations)
            {
                builder.RegisterType(registration.Type)
                    .As<IEventHandler>()
                    .WithMetadata(new List<KeyValuePair<string, object>>
                    {
                        new KeyValuePair<string, object>(Constants.MinionEventHandlerMetadataKeys.MinionKind,
                            registration.Key.MinionKind),
                        new KeyValuePair<string, object>(Constants.MinionEventHandlerMetadataKeys.EventType,
                            registration.Key.EventType)
                    });
            }
        }
    }
}